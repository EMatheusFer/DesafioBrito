using System;
using System.Collections.Generic;

namespace CadastroProdutos
{
    public class Categoria
    {
        public int Id { get; set; }  
        public required string Descricao { get; set; } 
        public decimal MargemLucro { get; set; } 
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;  
        public DateTime? DataInativacao { get; set; }

        public List<Produto> Produtos { get; set; } = new(); 
    }
}