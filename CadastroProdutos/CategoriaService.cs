using System;
using System.Linq;

namespace CadastroProdutos
{
    public class CategoriaService
    {
        private readonly AppDbContext _db;

        public CategoriaService(AppDbContext db)
        {
            _db = db;
        }

        // ======================
        // CREATE
        // ======================
        public void CriarCategoria()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRAR CATEGORIA ===");

            Console.Write("Descrição: ");
            var descricao = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(descricao))
            {
                Console.WriteLine("Descrição inválida!");
                Console.ReadLine();
                return;
            }

            if (_db.Categorias.IgnoreQueryFilters()
                  .Any(c => c.Descricao.ToLower() == descricao.ToLower()))
            {
                Console.WriteLine("Categoria já existe!");
                Console.ReadLine();
                return;
            }

            decimal margemLucro;
            while (true)
            {
                Console.Write("Margem de lucro (ex: 0.2 para 20%): ");
                var input = Console.ReadLine()?.Trim();

                if (decimal.TryParse(input, out margemLucro) && margemLucro >= 0 && margemLucro <= 1)
                    break;

                Console.WriteLine("Valor inválido! Digite como decimal entre 0 e 1 (ex: 0.15).");
            }

            var categoria = new Categoria
            {
                Descricao = descricao,
                MargemLucro = margemLucro
            };

            _db.Categorias.Add(categoria);
            _db.SaveChanges();

            Console.WriteLine("Categoria cadastrada com sucesso!");
            Console.ReadLine();
        }

        // ======================
        // READ / LISTAGENS
        // ======================
        public void ListarCategoriasAtivas()
        {
            Console.Clear();
            Console.WriteLine("=== CATEGORIAS ATIVAS ===");

            var categorias = _db.Categorias.ToList();
            if (!categorias.Any())
            {
                Console.WriteLine("Nenhuma categoria ativa encontrada.");
            }
            else
            {
                foreach (var c in categorias)
                    Console.WriteLine($"{c.Id} - {c.Descricao} | Lucro: {c.MargemLucro:P}");
            }

            Console.ReadLine();
        }

        public void ListarCategoriasInativas()
        {
            Console.Clear();
            Console.WriteLine("=== CATEGORIAS INATIVAS ===");

            var categorias = _db.CategoriasInativas().ToList();
            if (!categorias.Any())
            {
                Console.WriteLine("Nenhuma categoria inativa encontrada.");
            }
            else
            {
                foreach (var c in categorias)
                    Console.WriteLine($"{c.Id} - {c.Descricao} | Inativada em: {c.DataInativacao}");
            }

            Console.ReadLine();
        }

        public void ListarCategoriasSemVinculo()
        {
            Console.Clear();
            Console.WriteLine("=== CATEGORIAS SEM PRODUTOS ===");

            var categorias = _db.Categorias
                .Where(c => !c.Produtos.Any(p => p.Ativo))
                .ToList();

            if (!categorias.Any())
            {
                Console.WriteLine("Nenhuma categoria sem vínculo encontrada.");
            }
            else
            {
                foreach (var c in categorias)
                    Console.WriteLine($"{c.Id} - {c.Descricao}");
            }

            Console.ReadLine();
        }

        public void ListarTodasCategorias()
        {
            Console.Clear();
            Console.WriteLine("=== TODAS AS CATEGORIAS ===");

            var categorias = _db.Categorias.IgnoreQueryFilters().ToList();
            foreach (var c in categorias)
                Console.WriteLine($"{c.Id} - {c.Descricao} | Ativo: {c.Ativo}");

            Console.ReadLine();
        }

        // ======================
        // UPDATE
        // ======================
        public void EditarCategoria()
        {
            Console.Clear();
            Console.WriteLine("=== EDITAR CATEGORIA ===");

            var categorias = _db.Categorias.IgnoreQueryFilters().ToList();
            if (!categorias.Any())
            {
                Console.WriteLine("Nenhuma categoria encontrada!");
                Console.ReadLine();
                return;
            }

            foreach (var cat in categorias)
                Console.WriteLine($"{cat.Id} - {cat.Descricao} | Ativo: {cat.Ativo}");

            Console.Write("Digite o ID da categoria que deseja editar: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("ID inválido!");
                Console.ReadLine();
                return;
            }

            var categoria = _db.Categorias.IgnoreQueryFilters().FirstOrDefault(c => c.Id == id);
            if (categoria == null)
            {
                Console.WriteLine("Categoria não encontrada!");
                Console.ReadLine();
                return;
            }

            // Editar descrição
            Console.Write($"Nova descrição ({categoria.Descricao}): ");
            var novaDescricao = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(novaDescricao))
                categoria.Descricao = novaDescricao;

            // Editar margem de lucro
            while (true)
            {
                Console.Write($"Nova margem ({categoria.MargemLucro}): ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                    break; // mantém valor atual

                if (decimal.TryParse(input, out var novaMargem) && novaMargem >= 0 && novaMargem <= 1)
                {
                    categoria.MargemLucro = novaMargem;
                    break;
                }

                Console.WriteLine("Valor inválido! Digite como decimal entre 0 e 1 (ex: 0.15) ou ENTER para manter.");
            }

            // Reativar categoria inativa
            if (!categoria.Ativo)
            {
                Console.Write("Essa categoria está inativa. Deseja reativá-la? (s/n): ");
                var resp = Console.ReadLine()?.Trim().ToLower();
                if (resp == "s")
                {
                    categoria.Ativo = true;
                    categoria.DataInativacao = null;
                    Console.WriteLine("Categoria reativada com sucesso!");
                }
            }

            _db.SaveChanges();
            Console.WriteLine("Categoria atualizada com sucesso!");
            Console.ReadLine();
        }

        // ======================
        // DELETE (LÓGICO)
        // ======================
        public void ExcluirCategoria()
        {
            Console.Clear();
            Console.WriteLine("=== EXCLUIR CATEGORIA (LÓGICO) ===");

            var categorias = _db.Categorias.ToList();
            if (!categorias.Any())
            {
                Console.WriteLine("Nenhuma categoria encontrada!");
                Console.ReadLine();
                return;
            }

            foreach (var cat in categorias)
                Console.WriteLine($"{cat.Id} - {cat.Descricao}");

            Console.Write("Digite o ID da categoria que deseja excluir: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("ID inválido!");
                Console.ReadLine();
                return;
            }

            var categoria = _db.Categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
            {
                Console.WriteLine("Categoria não encontrada!");
                Console.ReadLine();
                return;
            }

            if (_db.Produtos.Any(p => p.CategoriaId == categoria.Id && p.Ativo))
            {
                Console.WriteLine("Não é possível excluir, existem produtos vinculados!");
                Console.ReadLine();
                return;
            }

            categoria.Ativo = false;
            categoria.DataInativacao = DateTime.Now;
            _db.SaveChanges();

            Console.WriteLine("Categoria inativada com sucesso!");
            Console.ReadLine();
        }

        // ======================
        // BUSCAS
        // ======================
        public void BuscarCategoriaPorId()
        {
            Console.Clear();
            Console.WriteLine("=== BUSCAR CATEGORIA POR ID ===");

            Console.Write("Digite o ID: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("ID inválido!");
                Console.ReadLine();
                return;
            }

            var categoria = _db.Categorias.IgnoreQueryFilters().FirstOrDefault(c => c.Id == id);
            if (categoria == null)
                Console.WriteLine("Categoria não encontrada!");
            else
                Console.WriteLine($"{categoria.Id} - {categoria.Descricao} | Ativo: {categoria.Ativo}");

            Console.ReadLine();
        }

        public void BuscarCategoriaPorNome()
        {
            Console.Clear();
            Console.WriteLine("=== BUSCAR CATEGORIA POR NOME ===");

            Console.Write("Digite parte do nome: ");
            var nome = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(nome))
            {
                Console.WriteLine("Entrada inválida!");
                Console.ReadLine();
                return;
            }

            var categorias = _db.Categorias.IgnoreQueryFilters()
                .Where(c => c.Descricao.ToLower().Contains(nome.ToLower()))
                .ToList();

            if (!categorias.Any())
                Console.WriteLine("Nenhuma categoria encontrada!");
            else
                foreach (var c in categorias)
                    Console.WriteLine($"{c.Id} - {c.Descricao} | Ativo: {c.Ativo}");

            Console.ReadLine();
        }
    }
}