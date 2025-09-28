using System;
using System.Collections.Generic;


namespace CadastroProdutos
{
    public class Produto
    {
        public int Id { get; set; }

        public required string SKU { get; set; }     

        public required string Nome { get; set; }      

        public decimal PrecoCusto { get; set; }        

        public decimal PrecoVenda { get; private set; } 
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public bool Ativo { get; set; } = true;

        public DateTime? DataInativacao { get; set; }

    
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

       
        public void CalcularPrecoVenda()
        {
            if (Categoria == null)
                throw new InvalidOperationException("Produto não possuí categoria.");

            PrecoVenda = PrecoCusto * (1 + Categoria.MargemLucro);
        }
    }
}