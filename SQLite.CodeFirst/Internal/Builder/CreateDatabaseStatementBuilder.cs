﻿using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Statement;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Builder
{
    internal class CreateDatabaseStatementBuilder : IStatementBuilder<CreateDatabaseStatement>
    {
        private readonly EdmModel edmModel;
        private readonly Collation defaultCollation;

        public CreateDatabaseStatementBuilder(EdmModel edmModel, Collation defaultCollation)
        {
            this.edmModel = edmModel;
            this.defaultCollation = defaultCollation;
        }

        public CreateDatabaseStatement BuildStatement()
        {
            var createTableStatements = GetCreateTableStatements();
            var createIndexStatements = GetCreateIndexStatements();
            var createStatements = createTableStatements.Concat<IStatement>(createIndexStatements);
            var createDatabaseStatement = new CreateDatabaseStatement(createStatements);
            return createDatabaseStatement;
        }

        private IEnumerable<CreateTableStatement> GetCreateTableStatements()
        {
            var associationTypeContainer = new AssociationTypeContainer(edmModel.AssociationTypes, edmModel.Container);

            foreach (var entitySet in edmModel.Container.EntitySets)
            {
                var tableStatementBuilder = new CreateTableStatementBuilder(entitySet, associationTypeContainer, defaultCollation);
                yield return tableStatementBuilder.BuildStatement();
            }
        }

        private IEnumerable<CreateIndexStatementCollection> GetCreateIndexStatements()
        {
            foreach (var entitySet in edmModel.Container.EntitySets)
            {
                var indexStatementBuilder = new CreateIndexStatementBuilder(entitySet);
                yield return indexStatementBuilder.BuildStatement();
            }
        }
    }
}