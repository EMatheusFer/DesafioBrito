using System;
using System.Linq;

namespace CadastroProdutos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new AppDbContext();
            bool rodando = true;

            while (rodando)
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
                Console.WriteLine("8 - Sair");
                Console.Write("Escolha uma opção: ");

                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        MostrarMenuCadastros(db);
                        break;
                    case "2":
                        MostrarMenuEdicao(db);
                        break;
                    case "3":
                        MostrarMenuExclusao(db);
                        break;
                    case "4":
                        MostrarMenuListagem(db);
                        break;
                    case "5":
                        MostrarMenuBusca(db);
                        break;
                    case "6":
                        ExportarProdutosCSV(db);
                        break;
                    case "7":
                        ExportarCategoriasCSV(db);
                        break;
                    case "8":
                        rodando = false;
                        break;
                    default:
                        Console.WriteLine("Opção inválida! Pressione ENTER para continuar.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        static void MostrarMenuCadastros(AppDbContext db)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== CADASTROS ===");
                Console.WriteLine("1 - Categorias");
                Console.WriteLine("2 - Produtos");
                Console.WriteLine("3 - Voltar");
                Console.Write("Escolha uma opção: ");

                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        CadastrarCategoria(db);
                        break;
                    case "2":
                        CadastrarProduto(db);
                        break;
                    case "3":
                        return; 
                    default:
                        Console.WriteLine("Opção inválida! Pressione ENTER para continuar.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        static void CadastrarCategoria(AppDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRAR CATEGORIA ===");

        
            Console.Write("Descrição: ");
            var descricao = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(descricao))
            {
                Console.WriteLine("Descrição inválida! Pressione ENTER para voltar.");
                Console.ReadLine();
                return;
            }

         
            if (db.Categorias.IgnoreQueryFilters().Any(c => c.Descricao.ToLower() == descricao.ToLower()))
            {
                Console.WriteLine("Categoria já existe! Pressione ENTER para voltar.");
                Console.ReadLine();
                return;
            }

            
            decimal margemLucro;
            while (true)
            {
                Console.Write("Margem de lucro (ex: 0.2 para 20%): ");
                var input = Console.ReadLine()?.Trim();

                if (decimal.TryParse(input, out margemLucro) && margemLucro >= 0)
                {
                    break; 
                }
                else
                {
                    Console.WriteLine("Valor inválido! Digite como decimal (ex: 0.15 para 15%).");
                }
            }

            
            var categoria = new Categoria
            {
                Descricao = descricao,
                MargemLucro = margemLucro
            };

            db.Categorias.Add(categoria);
            db.SaveChanges();

            Console.WriteLine("Categoria cadastrada com sucesso! Pressione ENTER.");
            Console.ReadLine();
        }
        static void CadastrarProduto(AppDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRAR PRODUTO ===");

            Console.Write("SKU: ");
            var sku = Console.ReadLine()?.Trim();

            Console.Write("Nome: ");
            var nome = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(sku) || string.IsNullOrEmpty(nome))
            {
                Console.WriteLine("SKU ou Nome inválido! Pressione ENTER.");
                Console.ReadLine();
                return;
            }

            // Verifica duplicidade (RN02 e RN03)
            if (db.Produtos.IgnoreQueryFilters().Any(p => p.SKU.ToLower() == sku.ToLower()))
            {
                Console.WriteLine("SKU já cadastrado! Pressione ENTER.");
                Console.ReadLine();
                return;
            }
            if (db.Produtos.IgnoreQueryFilters().Any(p => p.Nome.ToLower() == nome.ToLower()))
            {
                Console.WriteLine("Produto com este nome já existe! Pressione ENTER.");
                Console.ReadLine();
                return;
            }

            Console.Write("Preço de custo: ");
            if (!decimal.TryParse(Console.ReadLine(), out var precoCusto))
            {
                Console.WriteLine("Preço inválido! Pressione ENTER.");
                Console.ReadLine();
                return;
            }

            // Escolher categoria
            var categorias = db.Categorias.ToList();
            if (!categorias.Any())
            {
                Console.WriteLine("Não há categorias cadastradas. Pressione ENTER.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Selecione a categoria:");
            for (int i = 0; i < categorias.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {categorias[i].Descricao}");
            }

            if (!int.TryParse(Console.ReadLine(), out var indice) || indice < 1 || indice > categorias.Count)
            {
                Console.WriteLine("Categoria inválida! Pressione ENTER.");
                Console.ReadLine();
                return;
            }

            var produto = new Produto
            {
                SKU = sku,
                Nome = nome,
                PrecoCusto = precoCusto,
                CategoriaId = categorias[indice - 1].Id
            };

            produto.CalcularPrecoVenda(); // RN05

            db.Produtos.Add(produto);
            db.SaveChanges();

            Console.WriteLine("Produto cadastrado com sucesso! Pressione ENTER.");
            Console.ReadLine();
        }
    }
}