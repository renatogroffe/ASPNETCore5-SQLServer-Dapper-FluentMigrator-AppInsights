using System;
using FluentMigrator;

namespace APIIndicadores.Migrations
{
    [Migration(2021_01_01_001)]
    public class IndicadoresMigration_v2021_01_01_001 : Migration
    {
        public override void Up()
        {
    		Create.Table("Indicadores")
	    		.WithColumn("Sigla").AsAnsiString(10).NotNullable().PrimaryKey()
		    	.WithColumn("NomeIndicador").AsAnsiString(60).NotNullable()
                .WithColumn("UltimaAtualizacao").AsDateTime().NotNullable()
                .WithColumn("Valor").AsDecimal(18, 4).NotNullable();

            InsertIndicador("SALARIO", "Salario minimo",
                new DateTime(2020, 01, 01), 1045);
            InsertIndicador("IPCA", "Indice Nacional de Precos ao Consumidor Amplo",
                new DateTime(2020, 11, 30), 0.0089);
            InsertIndicador("SELIC", "Taxa Referencial - Sistema de Liquidacao e Custodia",
                new DateTime(2020, 12, 09), 0.02);
        }

        private void InsertIndicador(
            string sigla, string nomeIndicador,
            DateTime ultimaAtualizacao, double valor)
        {
            Insert.IntoTable("Indicadores").Row(new
            {
                Sigla = sigla,
                NomeIndicador = nomeIndicador,
                UltimaAtualizacao = ultimaAtualizacao,
                Valor = valor
            });
        }

        public override void Down()
        {
            Delete.Table("Indicadores");
        }
    }
}