
namespace BasePessoa
{
    class IdentificarPessoas
    {
        static void Main()
        {
            string arquivo = "basepessoa.csv";

            using StreamReader leitor = new(arquivo);
            string? linha;
            string[] colunas = { "Código", "Nome", "CPF", "Data de Nascimento" };

            while ((linha = leitor.ReadLine()) != null)
            {
                string[] tokens = linha.Split(',');

                for (int i = 0; i < tokens.Length && i < colunas.Length; i++)
                {
                    Console.WriteLine($"{colunas[i]}: {tokens[i]}");
                }
                Console.WriteLine("-----------------------------");
            }

            int total = ObterTotalPessoas(arquivo);
            Console.WriteLine($"Total de pessoas: {total}");

            Console.WriteLine("-----------------------------");

            DateTime dataReferencia = new DateTime(1987, 1, 1);
            var pessoasDepois1987 = ObterPessoasAPartirDe(arquivo, dataReferencia);

            Console.WriteLine($"Pessoas nascidas a partir de {dataReferencia:dd/MM/yyyy}:");
            foreach (var p in pessoasDepois1987)
            {
                Console.WriteLine($"{p.nome} - {p.nascimento:dd/MM/yyyy}");
            }
            Console.WriteLine("-----------------------------");

            List<string> nomesComM = ObterNomesQueComecamComM(arquivo);
            Console.WriteLine("Nomes que começam com M:");
            foreach (var nome in nomesComM)
                Console.WriteLine(nome);
            Console.WriteLine("-----------------------------");

            double mediaIdade = CalcularMediaIdade(arquivo);
            Console.WriteLine($"Média de idade das pessoas: {(int)mediaIdade} anos");
            Console.WriteLine("-----------------------------");
        }
        static int ObterTotalPessoas(string caminhoArquivo)
        {
            using StreamReader leitor = new(caminhoArquivo);
            string conteudo = leitor.ReadToEnd();
            string[] linhas = conteudo.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            return linhas.Length;
        }
        static List<(string nome, DateTime nascimento)> ObterPessoasAPartirDe(string caminhoArquivo, DateTime data)
        {
            var pessoas = new List<(string nome, DateTime nascimento)>();

            using (StreamReader leitor = new StreamReader(caminhoArquivo))
            {
                string linha;
                while ((linha = leitor.ReadLine()) != null)
                {
                    string[] tokens = linha.Split(',');

                    // tokens[1] = nome, tokens[3] = data de nascimento
                    if (tokens.Length > 3 && DateTime.TryParse(tokens[3], out DateTime dataNascimento))
                    {
                        if (dataNascimento >= data)
                            pessoas.Add((tokens[1], dataNascimento));
                    }
                }
            }

            return pessoas;
        }

        static List<string> ObterNomesQueComecamComM(string caminhoArquivo)
        {
            List<string> nomes = new List<string>();
            using (var leitor = new StreamReader(caminhoArquivo))
            {
                string linha;
                while ((linha = leitor.ReadLine()) != null)
                {
                    string[] tokens = linha.Split(',');
                    if (tokens.Length > 1 && tokens[1].StartsWith("M", StringComparison.OrdinalIgnoreCase))
                        nomes.Add(tokens[1]);
                }
            }
            return nomes;
        }

        static double CalcularMediaIdade(string caminhoArquivo)
        {
            int somaIdades = 0;
            int total = 0;

            using (StreamReader leitor = new StreamReader(caminhoArquivo))
            {
                string linha;
                while ((linha = leitor.ReadLine()) != null)
                {
                    string[] tokens = linha.Split(',');
                    if (tokens.Length > 3 && DateTime.TryParse(tokens[3], out DateTime dataNascimento))
                    {
                        // Calcula idade aproximada
                        int idade = DateTime.Now.Year - dataNascimento.Year;
                        // Ajusta se ainda não fez aniversário esse ano
                        if (DateTime.Now < dataNascimento.AddYears(idade))
                            idade--;

                        somaIdades += idade;
                        total++;
                    }
                }
            }

            return total > 0 ? (double)somaIdades / total : 0;
        }
    }
}