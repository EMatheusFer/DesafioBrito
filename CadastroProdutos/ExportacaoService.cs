using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CadastroProdutos
{
    public class ExportService
    {
        private readonly AppDbContext _db;

        public ExportService(AppDbContext db)
        {
            _db = db;
        }

   
        public void ExportarProdutosParaCsv()
        {
            Console.Clear();
            Console.WriteLine("=== EXPORTAR PRODUTOS PARA CSV ===");

            var produtos = _db.Produtos
                .IgnoreQueryFilters()
                .Include(p => p.Categoria)
                .ToList();

            if (!produtos.Any())
            {
                Console.WriteLine("Nenhum produto encontrado para exportação.");
                Console.ReadLine();
                return;
            }

            try
            {
                string diretorio = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(diretorio, "produtos.csv");

                using var writer = new StreamWriter(path, false, System.Text.Encoding.UTF8);

                writer.WriteLine("Id;SKU;Nome;PrecoCusto;PrecoVenda;DataCadastro;Ativo;DataInativacao;CategoriaId;CategoriaDescricao;MargemLucro");

                foreach (var p in produtos)
                {
                    var categoria = p.Categoria;
                    string linha = string.Join(';',
                        p.Id,
                        EscapeCsv(p.SKU),
                        EscapeCsv(p.Nome),
                        p.PrecoCusto.ToString("F2", CultureInfo.GetCultureInfo("pt-BR")),
                        p.PrecoVenda.ToString("F2", CultureInfo.GetCultureInfo("pt-BR")),
                        p.DataCadastro.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("pt-BR")),
                        p.Ativo ? "Sim" : "Não",
                        p.DataInativacao.HasValue
                            ? p.DataInativacao.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("pt-BR"))
                            : "",
                        p.CategoriaId,
                        EscapeCsv(categoria?.Descricao ?? ""),
                        categoria?.MargemLucro.ToString("P2", CultureInfo.GetCultureInfo("pt-BR")) ?? ""
                    );

                    writer.WriteLine(linha);
                }

                writer.Flush();

                Console.WriteLine($"\nArquivo 'produtos.csv' gerado com sucesso!");
                Console.WriteLine($"Local: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar: {ex.Message}");
            }

            Console.ReadLine();
        }

     
        public void ExportarCategoriasParaCsv()
        {
            Console.Clear();
            Console.WriteLine("=== EXPORTAR CATEGORIAS PARA CSV ===");

            var categorias = _db.Categorias
                .IgnoreQueryFilters()
                .Include(c => c.Produtos)
                .ToList();

            if (!categorias.Any())
            {
                Console.WriteLine("Nenhuma categoria encontrada para exportação.");
                Console.ReadLine();
                return;
            }

            try
            {
                string diretorio = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(diretorio, "categorias.csv");

                using var writer = new StreamWriter(path, false, System.Text.Encoding.UTF8);

                writer.WriteLine("Id;Descricao;MargemLucro;Ativo;DataCadastro;DataInativacao;QtdProdutos");

                foreach (var c in categorias)
                {
                    string linha = string.Join(';',
                        c.Id,
                        EscapeCsv(c.Descricao),
                        c.MargemLucro.ToString("P2", CultureInfo.GetCultureInfo("pt-BR")),
                        c.Ativo ? "Sim" : "Não",
                        c.DataCadastro.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("pt-BR")),
                        c.DataInativacao.HasValue
                            ? c.DataInativacao.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("pt-BR"))
                            : "",
                        c.Produtos?.Count ?? 0
                    );

                    writer.WriteLine(linha);
                }

                writer.Flush();

                Console.WriteLine($"\nArquivo 'categorias.csv' gerado com sucesso!");
                Console.WriteLine($"Local: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar: {ex.Message}");
            }

            Console.ReadLine();
        }

 
        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(';') || value.Contains('"'))
                return $"\"{value.Replace("\"", "\"\"")}\"";

            return value;
        }
    }
}