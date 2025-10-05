using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore; 

namespace CadastroProdutos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new AppDbContext();
           
            var categoriaService = new CategoriaService(db);
            var produtoService = new ProdutoService(db);
            var exportService = new ExportService(db);

            bool sair = false;

            while (!sair)
            {
                Console.Clear();
                Console.WriteLine("=== MENU PRINCIPAL ===");
                Console.WriteLine("1 - Cadastros");
                Console.WriteLine("2 - Edição");
                Console.WriteLine("3 - Exclusão");
                Console.WriteLine("4 - Listagem");
                Console.WriteLine("5 - Busca");
                Console.WriteLine("6 - Exportar Produtos CSV");
                Console.WriteLine("7 - Exportar Categorias CSV");
                Console.WriteLine("0 - Sair");
                Console.Write("\nEscolha uma opção: ");

                string? opc = Console.ReadLine()?.Trim();

                switch (opc)
                {
                    case "1":
                        MenuCadastros(categoriaService, produtoService);
                        break;
                    case "2":
                        MenuEdicao(categoriaService, produtoService);
                        break;
                    case "3":
                        MenuExclusao(categoriaService, produtoService);
                        break;
                    case "4":
                        MenuListagem(categoriaService, produtoService);
                        break;
                    case "5":
                        MenuBusca(categoriaService, produtoService);
                        break;
                    case "6":
                        exportService.ExportarProdutosParaCsv();
                        break;
                    case "7":
                        exportService.ExportarCategoriasParaCsv();
                        break;
                    case "0":
                        sair = true;
                        break;
                    default:
                        MensagemVoltar("Opção inválida!");
                        break;
                }
            }

            Console.WriteLine("\nSaindo do sistema... Até logo!");
        }

        // ---------------- MENU DE CADASTROS ----------------
        private static void MenuCadastros(CategoriaService categoriaService, ProdutoService produtoService)
        {
            bool voltar = false;

            while (!voltar)
            {
                Console.Clear();
                Console.WriteLine("=== CADASTROS ===");
                Console.WriteLine("1 - Categorias");
                Console.WriteLine("2 - Produtos");
                Console.WriteLine("0 - Voltar");
                Console.Write("\nEscolha: ");
                string? opc = Console.ReadLine()?.Trim();

                switch (opc)
                {
                    case "1":
                        categoriaService.CriarCategoria();
                        break;
                    case "2":
                        produtoService.CriarProduto();
                        break;
                    case "0":
                        voltar = true;
                        break;
                    default:
                        MensagemVoltar("Opção inválida!");
                        break;
                }
            }
        }

        // ---------------- MENU DE EDIÇÃO ----------------
        private static void MenuEdicao(CategoriaService categoriaService, ProdutoService produtoService)
        {
            bool voltar = false;

            while (!voltar)
            {
                Console.Clear();
                Console.WriteLine("=== EDIÇÃO ===");
                Console.WriteLine("1 - Categorias");
                Console.WriteLine("2 - Produtos");
                Console.WriteLine("0 - Voltar");
                Console.Write("\nEscolha: ");
                string? opc = Console.ReadLine()?.Trim();

                switch (opc)
                {
                    case "1":
                        categoriaService.EditarCategoria();
                        break;
                    case "2":
                        produtoService.EditarProduto();
                        break;
                    case "0":
                        voltar = true;
                        break;
                    default:
                        MensagemVoltar("Opção inválida!");
                        break;
                }
            }
        }

        // ---------------- MENU DE EXCLUSÃO ----------------
        private static void MenuExclusao(CategoriaService categoriaService, ProdutoService produtoService)
        {
            bool voltar = false;

            while (!voltar)
            {
                Console.Clear();
                Console.WriteLine("=== EXCLUSÃO ===");
                Console.WriteLine("1 - Categorias");
                Console.WriteLine("2 - Produtos");
                Console.WriteLine("0 - Voltar");
                Console.Write("\nEscolha: ");
                string? opc = Console.ReadLine()?.Trim();

                switch (opc)
                {
                    case "1":
                        categoriaService.ExcluirCategoria();
                        break;
                    case "2":
                        produtoService.ExcluirProduto();
                        break;
                    case "0":
                        voltar = true;
                        break;
                    default:
                        MensagemVoltar("Opção inválida!");
                        break;
                }
            }
        }

        // ---------------- MENU DE LISTAGEM ----------------
        private static void MenuListagem(CategoriaService categoriaService, ProdutoService produtoService)
        {
            bool voltar = false;

            while (!voltar)
            {
                Console.Clear();
                Console.WriteLine("=== LISTAGENS ===");
                Console.WriteLine("1 - Categorias Ativas");
                Console.WriteLine("2 - Categorias Inativas");
                Console.WriteLine("3 - Categorias sem vínculo");
                Console.WriteLine("4 - Todas as Categorias");
                Console.WriteLine("5 - Produtos Ativos");
                Console.WriteLine("6 - Produtos Inativos");
                Console.WriteLine("7 - Produtos sem Categoria");
                Console.WriteLine("8 - Todos os Produtos");
                Console.WriteLine("0 - Voltar");
                Console.Write("\nEscolha: ");
                string? opc = Console.ReadLine()?.Trim();

                switch (opc)
                {
                    case "1": categoriaService.ListarCategoriasAtivas(); break;
                    case "2": categoriaService.ListarCategoriasInativas(); break;
                    case "3": categoriaService.ListarCategoriasSemVinculo(); break;
                    case "4": categoriaService.ListarTodasCategorias(); break;
                    case "5": produtoService.ListarProdutosAtivos(); break;
                    case "6": produtoService.ListarProdutosInativos(); break;
                    case "7": produtoService.ListarProdutosSemCategoria(); break;
                    case "8": produtoService.ListarTodosProdutos(); break;
                    case "0": voltar = true; break;
                    default: MensagemVoltar("Opção inválida!"); break;
                }
            }
        }

        // ---------------- MENU DE BUSCA ----------------
        private static void MenuBusca(CategoriaService categoriaService, ProdutoService produtoService)
        {
            bool voltar = false;

            while (!voltar)
            {
                Console.Clear();
                Console.WriteLine("=== BUSCAS ===");
                Console.WriteLine("1 - Categoria por código");
                Console.WriteLine("2 - Categoria por nome");
                Console.WriteLine("3 - Produto por SKU");
                Console.WriteLine("4 - Produto por nome");
                Console.WriteLine("0 - Voltar");
                Console.Write("\nEscolha: ");
                string? opc = Console.ReadLine()?.Trim();

                switch (opc)
                {
                    case "1": categoriaService.BuscarCategoriaPorId(); break;
                    case "2": categoriaService.BuscarCategoriaPorNome(); break;
                    case "3": produtoService.BuscarProdutoPorSku(); break;
                    case "4": produtoService.BuscarProdutoPorNome(); break;
                    case "0": voltar = true; break;
                    default: MensagemVoltar("Opção inválida!"); break;
                }
            }
        }

        // ---------------- UTILITÁRIO ----------------
        private static void MensagemVoltar(string mensagem)
        {
            Console.WriteLine($"\n{mensagem}");
            Console.WriteLine("Pressione ENTER para continuar...");
            Console.ReadLine();
        }
    }
}