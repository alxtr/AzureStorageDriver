//-----------------------------------------------------------------------
// <copyright file="SchemaBuilder.cs" company="madd0.com">
//     Copyright (c) 2012 Mauricio DIAZ ORLICH.
//     Code licensed under the MIT X11 license.
// </copyright>
// <author>Mauricio DIAZ ORLICH</author>
//-----------------------------------------------------------------------

using Azure;
using Azure.Data.Tables;
using LINQPad;
using LINQPad.Extensibility.DataContext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Madd0.AzureStorageDriver
{
    /// <summary>
    /// Provides the methods necessary to determining the storage account's schema and to building
    /// the typed data context .
    /// </summary>
    internal static class SchemaBuilder
    {
        // Names of columns that should be marked as table keys.
        private static readonly List<string> KeyColumns = new List<string>
        {
            "PartitionKey", "RowKey"
        };

        /// <summary>
        /// Gets the schema and builds the assembly.
        /// </summary>
        /// <param name="properties">The current configuration.</param>
        /// <param name="name">The <see cref="AssemblyName"/> instace of the assembly being created.</param>
        /// <param name="namepace">The namespace to be used in the generated code.</param>
        /// <param name="typeName">Name of the type of the typed data context.</param>
        /// <returns>A list of <see cref="ExplorerItem"/> instaces that describes the current schema.</returns>
        public static List<ExplorerItem> GetSchemaAndBuildAssembly(StorageAccountProperties properties, AssemblyName name, string @namepace, string typeName)
        {
            // Get the model from Azure storage
            var model = GetModel(properties);

            // Generate C# code
            var code = GenerateCode(typeName, @namepace, model);

            // And compile the code into the assembly
            BuildAssembly(name, code, properties.ConnectionInfo);

            // Generate the schema for LINQPad
            List<ExplorerItem> schema = GetSchema(model);

            return schema;
        }

        /// <summary>
        /// Build a model of the current Azure storage account. This model will be used to generate
        /// the typed code as well as the schema needed by LINQPad.
        /// </summary>
        /// <param name="properties">The current configuration.</param>
        /// <returns>A list of <see cref="CloudTable"/> instances that describe the current Azure
        /// storage model.</returns>
        private static IEnumerable<CloudTable> GetModel(StorageAccountProperties properties)
        {
            // make sure that we can make at least ModelLoadMaxParallelism concurrent
            // cals to azure table storage
            ServicePointManager.DefaultConnectionLimit = properties.ModelLoadMaxParallelism;

            var tableClient = properties.GetStorageAccount();

            // First get a list of all tables
            var model = (from tableName in tableClient.Query()
                select new CloudTable
                {
                    Name = tableName.Name
                }).ToList();

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = properties.ModelLoadMaxParallelism
            };

            Parallel.ForEach(model, options, table =>
            {
                var threadTableClient = properties.GetStorageAccount().GetTableClient(table.Name);

                var tableColumns = threadTableClient.Query<TableEntity>()
                    .Take(properties.NumberOfRows)
                    .SelectMany(x => x)
                    .GroupBy(x => x.Key)
                    .Where(x => !"odata.etag".Equals(x.Key))
                    .Select(x => new TableColumn
                    {
                        Name = x.Key, TypeName = x.First().Value.GetType().FullName
                    });

                table.Columns = tableColumns.ToArray();
            });

            return model;
        }

        /// <summary>
        /// Generates the code to build the typed data context.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="model">The model.</param>
        /// <returns>The code to be compiled as a string.</returns>
        private static string GenerateCode(string typeName, string @namespace, IEnumerable<CloudTable> model)
        {
            // We use a T4-generated class as the template
            var codeGenerator = new DataContextTemplate
            {
                Namespace = @namespace, TypeName = typeName, Tables = model
            };

            // As a workaround for compiling issues when referencing the driver DLL itself to
            // access the base ExtendedTableQuery class, the code will be used instead in the
            // dynamic assembly.
            var baseClass = ReadEmbeddedBaseClassCode();

            var sourceCode = baseClass + codeGenerator.TransformText();
            Util.Break();

            return sourceCode;
        }

        private static string ReadEmbeddedBaseClassCode()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Madd0.AzureStorageDriver.ExtendedTableQuery.cs";

            using var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName));
        
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Builds the assembly described by the <see cref="AssemblyName"/>.
        /// </summary>
        /// <param name="name">The <see cref="AssemblyName"/> instace of the assembly being created.</param>
        /// <param name="code">The code of the typed data context.</param>
        private static void BuildAssembly(AssemblyName name, string code, IConnectionInfo connectionInfo)
        {
            ICompiler compiler;

            string[] dependencies = {
                typeof(TableClient).Assembly.Location,
                typeof(ETag).Assembly.Location
            };

            compiler = new RoslynCompiler();
            compiler.Compile(connectionInfo, code, name, dependencies);
        }

        /// <summary>
        /// Transforms the model based on <see cref="CloudTable"/> instances into a schema based on
        /// <see cref="ExplorerItem"/> instances for LINQPad.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A schema for LINQPad.</returns>
        private static List<ExplorerItem> GetSchema(IEnumerable<CloudTable> model)
        {
            return (from table in model
                select new ExplorerItem(table.Name, ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
                {
                    Children = (from column in table.Columns
                        select new ExplorerItem(column.Name + " (" + column.TypeName + ")", ExplorerItemKind.Property, ExplorerIcon.Column)
                        {
                            Icon = KeyColumns.Contains(column.Name) ? ExplorerIcon.Key : ExplorerIcon.Column, DragText = column.Name
                        }).ToList(),
                    DragText = table.Name,
                    IsEnumerable = true
                }).ToList();
        }
    }
}
