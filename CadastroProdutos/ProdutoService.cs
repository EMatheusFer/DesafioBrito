using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CadastroProdutos
{
    public class ProdutoService
    {
        private readonly AppDbContext _db;

        public ProdutoService(AppDbContext db)
        {
            _db = db;
        }

        public void CriarProduto()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRAR PRODUTO ===");

            Console.Write("SKU (obrigatório): ");
            var sku = Console.ReadLine()?.Trim();
            Console.Write("Nome/Descrição (obrigatório): ");
            var nome = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(sku) || string.IsNullOrEmpty(nome))
            {
                Console.WriteLine("SKU e Nome são obrigatórios!");
                Console.ReadLine();
                return;
            }

            
            if (_db.Produtos.IgnoreQueryFilters().Any(p => p.SKU.ToLower() == sku.ToLower()))
            {
                Console.WriteLine("SKU já cadastrado!");
                Console.ReadLine();
                return;
            }

            
            if (_db.Produtos.IgnoreQueryFilters().Any(p => p.Nome.ToLower() == nome.ToLower()))
            {
                Console.WriteLine("Produto com esse nome já existe!");
                Console.ReadLine();
                return;
            }

            Console.Write("Preço de compra: ");
            if (!decimal.TryParse(Console.ReadLine(), out var precoCompra) || precoCompra < 0)
            {
                Console.WriteLine("Preço inválido!");
                Console.ReadLine();
                return;
            }

            
            var categorias = _db.Categorias.ToList();
            if (!categorias.Any())
            {
                Console.WriteLine("Não há categorias cadastradas. Cadastre uma categoria primeiro.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Selecione a categoria (ID):");
            foreach (var c in categorias)
                Console.WriteLine($"{c.Id} - {c.Descricao} | Margem: {c.MargemLucro:P}");

            if (!int.TryParse(Console.ReadLine(), out var catId) || catId <= 0)
            {
                Console.WriteLine("Categoria inválida!");
                Console.ReadLine();
                return;
            }

            var categoria = _db.Categorias.FirstOrDefault(c => c.Id == catId);
            if (categoria == null)
            {
                Console.WriteLine("Categoria não encontrada!");
                Console.ReadLine();
                return;
            }

            var produto = new Produto
            {
                SKU = sku,
                Nome = nome,
                PrecoCusto = precoCompra,
                CategoriaId = categoria.Id,
                Categoria = categoria
            };


            try
            {
                produto.CalcularPrecoVenda();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Erro ao calcular preço de venda: " + ex.Message);
                Console.ReadLine();
                return;
            }

            _db.Produtos.Add(produto);
            _db.SaveChanges();

            Console.WriteLine("Produto cadastrado com sucesso!");
            Console.ReadLine();
        }


        public void ListarProdutosAtivos()
        {
            Console.Clear();
            Console.WriteLine("=== PRODUTOS ATIVOS ===");

            var produtos = _db.Produtos
                .Include(p => p.Categoria)
                .ToList();

            if (!produtos.Any())
            {
                Console.WriteLine("Nenhum produto ativo encontrado.");
            }
            else
            {
                foreach (var p in produtos)
                {
                    var catDesc = p.Categoria != null ? p.Categoria.Descricao : "(sem categoria)";
                    Console.WriteLine($"SKU: {p.SKU} | Nome: {p.Nome} | Custo: {p.PrecoCusto:C} | Venda: {p.PrecoVenda:C} | Cat: {catDesc}");
                }
            }

            Console.ReadLine();
        }

        public void ListarProdutosInativos()
        {
            Console.Clear();
            Console.WriteLine("=== PRODUTOS INATIVOS ===");

            var produtos = _db.ProdutosInativos()
                .Include(p => p.Categoria)
                .ToList();

            if (!produtos.Any())
            {
                Console.WriteLine("Nenhum produto inativo encontrado.");
            }
            else
            {
                foreach (var p in produtos)
                {
                    var catDesc = p.Categoria != null ? p.Categoria.Descricao : "(sem categoria)";
                    Console.WriteLine($"SKU: {p.SKU} | Nome: {p.Nome} | Venda: {p.PrecoVenda:C} | Cat: {catDesc} | Inativado: {p.DataInativacao}");
                }
            }

            Console.ReadLine();
        }

        public void ListarProdutosSemCategoria()
        {
            Console.Clear();
            Console.WriteLine("=== PRODUTOS SEM CATEGORIA ===");

            var produtos = _db.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Categoria == null
                            || !_db.Categorias.IgnoreQueryFilters().Any(c => c.Id == p.CategoriaId))
                .ToList();

            if (!produtos.Any())
            {
                Console.WriteLine("Nenhum produto sem categoria encontrado.");
            }
            else
            {
                foreach (var p in produtos)
                    Console.WriteLine($"SKU: {p.SKU} | Nome: {p.Nome} | Custo: {p.PrecoCusto:C} | Venda: {p.PrecoVenda:C}");
            }

            Console.ReadLine();
        }

        public void ListarTodosProdutos()
        {
            Console.Clear();
            Console.WriteLine("=== TODOS OS PRODUTOS (ATIVOS E INATIVOS) ===");

            var produtos = _db.Produtos
                .IgnoreQueryFilters()
                .Include(p => p.Categoria)
                .ToList();

            if (!produtos.Any())
            {
                Console.WriteLine("Nenhum produto cadastrado.");
            }
            else
            {
                foreach (var p in produtos)
                {
                    var catDesc = p.Categoria != null ? p.Categoria.Descricao : "(sem categoria)";
                    Console.WriteLine($"SKU: {p.SKU} | Nome: {p.Nome} | Preço Venda: {p.PrecoVenda:C} | Cat: {catDesc} | Ativo: {p.Ativo}");
                }
            }

            Console.ReadLine();
        }


        public void EditarProduto()
        {
            Console.Clear();
            Console.WriteLine("=== EDITAR PRODUTO (LOCALIZAR POR SKU) ===");
            Console.Write("Digite o SKU do produto que deseja editar: ");
            var skuInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(skuInput))
            {
                Console.WriteLine("SKU inválido!");
                Console.ReadLine();
                return;
            }

            var produto = _db.Produtos
                .IgnoreQueryFilters()
                .Include(p => p.Categoria)
                .FirstOrDefault(p => p.SKU.ToLower() == skuInput.ToLower());

            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Produto encontrado: SKU: {produto.SKU} | Nome: {produto.Nome} | Ativo: {produto.Ativo}");
            Console.WriteLine("Deixe em branco (ENTER) para manter o valor atual.");

            // Alterar SKU
            Console.Write($"Novo SKU ({produto.SKU}): ");
            var novoSku = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(novoSku) && !novoSku.Equals(produto.SKU, StringComparison.OrdinalIgnoreCase))
            {
                if (_db.Produtos.IgnoreQueryFilters().Any(p => p.SKU.ToLower() == novoSku.ToLower() && p.Id != produto.Id))
                {
                    Console.WriteLine("SKU já cadastrado por outro produto! Operação cancelada.");
                    Console.ReadLine();
                    return;
                }
                produto.SKU = novoSku;
            }

           
            Console.Write($"Novo Nome ({produto.Nome}): ");
            var novoNome = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(novoNome) && !novoNome.Equals(produto.Nome, StringComparison.OrdinalIgnoreCase))
            {
                if (_db.Produtos.IgnoreQueryFilters().Any(p => p.Nome.ToLower() == novoNome.ToLower() && p.Id != produto.Id))
                {
                    Console.WriteLine("Nome já cadastrado por outro produto! Operação cancelada.");
                    Console.ReadLine();
                    return;
                }
                produto.Nome = novoNome;
            }

           
            Console.Write($"Novo Preço de custo ({produto.PrecoCusto}): ");
            var precoInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(precoInput))
            {
                if (decimal.TryParse(precoInput, out var novoPreco) && novoPreco >= 0)
                    produto.PrecoCusto = novoPreco;
                else
                {
                    Console.WriteLine("Preço inválido! Operação cancelada.");
                    Console.ReadLine();
                    return;
                }
            }


            Console.Write("Deseja alterar a categoria? (s/n): ");
            var trocarCat = Console.ReadLine()?.Trim().ToLower();
            if (trocarCat == "s")
            {
                var categorias = _db.Categorias.ToList();
                if (!categorias.Any())
                {
                    Console.WriteLine("Nenhuma categoria disponível.");
                    Console.ReadLine();
                    return;
                }

                foreach (var c in categorias)
                    Console.WriteLine($"{c.Id} - {c.Descricao} | Margem: {c.MargemLucro:P}");

                Console.Write("Digite o ID da nova categoria: ");
                if (!int.TryParse(Console.ReadLine(), out var novoCatId))
                {
                    Console.WriteLine("ID inválido! Operação cancelada.");
                    Console.ReadLine();
                    return;
                }

                var novaCategoria = _db.Categorias.FirstOrDefault(c => c.Id == novoCatId);
                if (novaCategoria == null)
                {
                    Console.WriteLine("Categoria não encontrada! Operação cancelada.");
                    Console.ReadLine();
                    return;
                }

                produto.CategoriaId = novaCategoria.Id;
                produto.Categoria = novaCategoria;
            }


            try
            {

                if (produto.Categoria == null)
                    produto.Categoria = _db.Categorias.IgnoreQueryFilters().FirstOrDefault(c => c.Id == produto.CategoriaId);

                produto.CalcularPrecoVenda();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Não foi possível calcular o preço de venda: produto sem categoria válida.");
                Console.ReadLine();
                return;
            }

            if (trocarCat != "s" && produto.Categoria != null)
            {
                _db.Entry(produto.Categoria).State = EntityState.Unchanged;
            }


            if (!produto.Ativo)
            {
                Console.Write("Esse produto está inativo. Deseja reativá-lo? (s/n): ");
                var resp = Console.ReadLine()?.Trim().ToLower();
                if (resp == "s")
                {
                    produto.Ativo = true;
                    produto.DataInativacao = null;
                    Console.WriteLine("Produto reativado.");
                }
            }

            _db.SaveChanges();
            Console.WriteLine("Produto atualizado com sucesso!");
            Console.ReadLine();
        }


        public void ExcluirProduto()
        {
            Console.Clear();
            Console.WriteLine("=== EXCLUIR PRODUTO (LOCALIZAR POR SKU) ===");
            Console.Write("Digite o SKU do produto que deseja excluir: ");
            var skuInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(skuInput))
            {
                Console.WriteLine("SKU inválido!");
                Console.ReadLine();
                return;
            }

            var produto = _db.Produtos.IgnoreQueryFilters().FirstOrDefault(p => p.SKU.ToLower() == skuInput.ToLower());
            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado.");
                Console.ReadLine();
                return;
            }

            if (!produto.Ativo)
            {
                Console.WriteLine("Produto já está inativo.");
                Console.ReadLine();
                return;
            }

            produto.Ativo = false;
            produto.DataInativacao = DateTime.UtcNow;
            _db.SaveChanges();

            Console.WriteLine("Produto inativado com sucesso!");
            Console.ReadLine();
        }


        public void BuscarProdutoPorSku()
        {
            Console.Clear();
            Console.WriteLine("=== BUSCAR PRODUTO POR SKU ===");
            Console.Write("Digite o SKU: ");
            var skuInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(skuInput))
            {
                Console.WriteLine("SKU inválido!");
                Console.ReadLine();
                return;
            }

            var produto = _db.Produtos
                .IgnoreQueryFilters()
                .Include(p => p.Categoria)
                .FirstOrDefault(p => p.SKU.ToLower() == skuInput.ToLower());

            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado.");
            }
            else
            {
                var catDesc = produto.Categoria != null ? produto.Categoria.Descricao : "(sem categoria)";
                Console.WriteLine($"SKU: {produto.SKU} | Nome: {produto.Nome} | Preço Venda: {produto.PrecoVenda:C} | Cat: {catDesc} | Ativo: {produto.Ativo}");
            }

            Console.ReadLine();
        }

        public void BuscarProdutoPorNome()
        {
            Console.Clear();
            Console.WriteLine("=== BUSCAR PRODUTO POR NOME (TRECHO) ===");
            Console.Write("Digite trecho do nome: ");
            var trecho = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(trecho))
            {
                Console.WriteLine("Entrada inválida!");
                Console.ReadLine();
                return;
            }

            var termo = trecho.ToLower();
            var produtos = _db.Produtos
                .IgnoreQueryFilters()
                .Include(p => p.Categoria)
                .Where(p => p.Nome.ToLower().Contains(termo))
                .ToList();

            if (!produtos.Any())
                Console.WriteLine("Nenhum produto encontrado.");
            else
                foreach (var p in produtos)
                {
                    var catDesc = p.Categoria != null ? p.Categoria.Descricao : "(sem categoria)";
                    Console.WriteLine($"SKU: {p.SKU} | Nome: {p.Nome} | Preço Venda: {p.PrecoVenda:C} | Cat: {catDesc} | Ativo: {p.Ativo}");
                }

            Console.ReadLine();
        }
    }
}