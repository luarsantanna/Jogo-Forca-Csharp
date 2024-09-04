using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.Wave;

namespace GAME
{
    public partial class Game : Form
    {
        public Game()
        {
            InitializeComponent();
        }

        WaveOutEvent Somgrito = new WaveOutEvent();

        string[] Lista;
        string[] Dicas; 
       

        Forca jogo;
        List<Label> Letras;
        int Erros = 0; //Numero de Erros do jogo

        SoundPlayer fundo; // Tocador de Som

        private void Game_Load(object sender, EventArgs e)
        {
            CarregarLista();
            NovoJogo();
            CarregadorFundoSonoro();
        }

        private void CarregarLista()
        {
            try
            {
                string arquivo = "lista.txt";
                string[] linhas = File.ReadAllLines(arquivo);
                Lista = new string[linhas.Count()];
                Dicas = new string[linhas.Count()];
                int i = 0;
                foreach(string linha in linhas)
                {
                    if (linha == string.Empty)
                    {
                        i--;
                        continue;
                    } 
                    Lista[i] = linha.Split(',')[0];
                    Dicas[i] = linha.Split(',')[1];
                    i++;
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro:" + erro.Message);
                Environment.Exit(0);
            }
        }

        private void CarregadorFundoSonoro()
        {
            fundo = new SoundPlayer();
            fundo.SoundLocation = "fundo.Wav";
            fundo.PlayLooping(); // tocando e voltando para o começo
        }

        private void NovoJogo()
        {
            lbLetras.Text = "";
            jogo = new Forca(Lista, Dicas);
            pnPalavra.Controls.Clear();
            Erros = 0;
            pbBoneco.Image = null;

            if (Lista.Count() !=Dicas.Count())
            {
                MessageBox.Show("Incosistencia no tamanho de lista ou dica");
                Environment.Exit(0);
            }
            jogo.Sortear(); // sorteia a palavra e a dica
            lbDica.Text = "Dica: " + jogo.getDica();
            DesenharPalavra();
        }

        private void DesenharPalavra()
        {
            string p = jogo.getPalavras();
            Letras = new List<Label>();
            int cx = 10;
            int cy = 10;
            for (int i = 0; i < p.Length; i++)
            {
                Label letra = new Label();
                letra.Text = "?";
                letra.Width = 40;
                letra.Height = 40;
                letra.AutoSize = false;
                letra.BorderStyle = BorderStyle.FixedSingle;
                letra.TextAlign = ContentAlignment.MiddleCenter;
                letra.ForeColor = Color.White;
                letra.BackColor = Color.Gray;
                Letras.Add(letra);
                if (i % 5 == 0 && i !=0)
                {
                    cy += 45;
                    cx = 10;
                }
                pnPalavra.Controls.Add(letra);
                letra.Top = cy;
                letra.Left = cx;
                cx += 45;
                letra.Show();
            }


        }

        private void btnJogar_Click(object sender, EventArgs e)
        {
            string letra = txtLetra.Text.ToUpper();
            ProcurarLetra(letra);
            txtLetra.Clear();
            txtLetra.Focus();
            Vitoria();
        }

        private void Vitoria()
        {
            string tmp = "";
            string p = jogo.getPalavras();
            foreach (Label i in Letras)
            {
                tmp += i.Text;
            }
            if (tmp.Equals(p))
            {
                cronometro.Stop();
                MessageBox.Show("Voce venceu!", "ALERTA",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                NovoJogo();
                lbCronometro.Text = "90";
                cronometro.Start();
            }
        }

        private void ProcurarLetra(string letra)
        {
            string p = jogo.getPalavras();
            bool achou = false;
            if (lbLetras.Text.Contains(letra))
            {
                MessageBox.Show("Letra já foi escolhida!", "Alerta", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            lbLetras.Text += " " + letra + " ";
            for (int i = 0; i < p.Length; i++)
            {
                if(p.Substring(i,1).Equals(letra))
                {
                    Letras[i].Text = letra;
                    Letras[i].ForeColor = Color.Gold;
                    Letras[i].BackColor = Color.Black;
                    achou = true;
                }
            }
            if (achou == false) DesenharBoneco();
        }

        private void DesenharBoneco()
        {
            Erros++; // incrementa o erro
            Gritar();
            string arquivo = "forca" +Erros +".png";
            if (Erros > 6) return;
            if(File.Exists(arquivo))
            {
                pbBoneco.Image = Image.FromFile(arquivo);
            }

            if (Erros == 6)
            {
                cronometro.Stop();
                Derrota();
                lbCronometro.Text = "90";
                cronometro.Start();
            }

        }

        private void Gritar()
        {
            string somFile = "grito.wav";
            AudioFileReader somReader = new AudioFileReader(somFile);
            Somgrito.Init(somReader);
            Somgrito.Play();

        }

        private void Derrota()
        {
            string p = jogo.getPalavras();
            MessageBox.Show("Voce perdeu. A palavra era " + p,
                "Alerta", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            NovoJogo();
        }

        private void cronometro_Tick(object sender, EventArgs e)
        {
            int seg = Convert.ToInt16(lbCronometro.Text);
            seg--;
            lbCronometro.Text = seg.ToString().PadLeft(2, '0');
            if (seg == 0)
            {
                cronometro.Stop();
                Derrota();
                lbCronometro.Text = "90";
                cronometro.Start();
            }
        }
        private void txtLetra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == Convert.ToChar(Keys.Enter)){
                btnJogar_Click(sender, e);
            }
        }
        
    }
}   

