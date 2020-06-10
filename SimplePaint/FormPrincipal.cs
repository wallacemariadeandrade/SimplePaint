using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimplePaint
{
    public partial class FormPrincipal : Form
    {
        private bool flagPintar = false; // Para controlar quando se deve pintar
        private Graphics graphicsPainelPintura;
        private float espessuraCaneta;
        private Color corBorracha;
        private bool flagApagar = false; // Para controlar quando se deve apagar com a borracha
        private Image imagemASalvar;
        private Graphics graphicsImagemASalvar;


        public FormPrincipal()
        {
            InitializeComponent();

            // As propriedades abaixo só fazem efeito quando o botão está com a propriedade FlatStyle
            // setada em Flat
            buttonBorracha.FlatAppearance.MouseOverBackColor = Color.DarkSlateGray; // altera a cor do botão quando o cursor está sobre ele
            buttonLimpar.FlatAppearance.MouseOverBackColor = Color.DarkSlateGray; // altera a cor do botão quando o cursor está sobre ele
            buttonSalvar.FlatAppearance.MouseOverBackColor = Color.DarkSlateGray; // altera a cor do botão quando o cursor está sobre ele

            for (int i=2; i <= 100; i+=2) // Irá preencher a comboBox de 2 até 100 contando de 2 em 2
            {
                comboBoxEspessuraDaCaneta.Items.Add(i);
            }
            comboBoxEspessuraDaCaneta.Text = "10"; // Texto que irá aparecer inicialmente na comboBox
            comboBoxEspessuraDaCaneta.IntegralHeight = false; // Necesário para que seja possível definir a quantidade de itens a serem exibidos na combobox
            comboBoxEspessuraDaCaneta.MaxDropDownItems = 5; // Define o número de itens a serem exibidos na combobox

            graphicsPainelPintura = panelPintura.CreateGraphics(); // O graphics permite o desenho sobre o controle
            espessuraCaneta = float.Parse(comboBoxEspessuraDaCaneta.Text); // Converte o texto da comboBox espessura para tipo float
            corBorracha = panelPintura.BackColor; // Especifica a cor padrão da borracha como a cor de fundo do painel

            imagemASalvar = new Bitmap(panelPintura.Width, panelPintura.Height); // imagem para salvar
            graphicsImagemASalvar = Graphics.FromImage(imagemASalvar); // extraindo graphics da imagem para salvar de forma a podermos desenhar nela
            graphicsImagemASalvar.Clear(panelPintura.BackColor); // Preenchemos a imagem com a cor do fundo do painel
        }

        // Handler do evento de click do botão
        // Serve para manipularmos o que acontece quando o clique ocorre
        private void buttonCorCaneta_Click(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog(); // ColorDialog é uma caixa de seleção de cores
            var corEscolhida = colorDialog.ShowDialog(); // Exibe na forma modal - aplicação fica travada enquanto o diálogo não é resolvido
            if(corEscolhida == DialogResult.OK) // Veriica se usuário clicou mesmo em OK
            {
                buttonCorCaneta.BackColor = colorDialog.Color; // Alteramos a cor do botão para a cor escolhida pelo usuário
            }
        }

        private void panelPintura_MouseDown(object sender, MouseEventArgs e)
        {
            flagPintar = true; // Quando usuário pressiona o botão do mouse sob o painel entendemos que ele quer pintar
        }

        private void panelPintura_MouseUp(object sender, MouseEventArgs e)
        {
            flagPintar = false; // Quando usuário solta o botão do mouse entendemos que ele não quer pintar
        }

        private void panelPintura_MouseMove(object sender, MouseEventArgs e)
        {
            if(flagPintar) // Só pintamos se o botão do mouse estiver pressionado! Como sabemos isso? Através da flag Pintar
            {
                if(!flagApagar)
                {
                    // Desenhamos uma elipse de cor e espessura definida pelo usuário, nas coordenadas do ponteiro do mouse
                    graphicsPainelPintura
                        .DrawEllipse(new Pen(buttonCorCaneta.BackColor, espessuraCaneta), new RectangleF(e.X, e.Y, espessuraCaneta, espessuraCaneta));
                    // Desenhando na imagem para salvar
                    graphicsImagemASalvar
                        .DrawEllipse(new Pen(buttonCorCaneta.BackColor, espessuraCaneta), new RectangleF(e.X, e.Y, espessuraCaneta, espessuraCaneta));
                }
                else
                {
                    // Desenhamos uma elipse de cor e espessura definida pelo usuário, nas coordenadas do ponteiro do mouse
                    graphicsPainelPintura
                        .DrawRectangle(new Pen(corBorracha, espessuraCaneta), new Rectangle(e.X, e.Y, (int)espessuraCaneta, (int)espessuraCaneta)); // Transformamos uma variável tipo float em tipo inteiro
                    // Desenhando na imagem para salvar
                    graphicsImagemASalvar
                        .DrawRectangle(new Pen(corBorracha, espessuraCaneta), new Rectangle(e.X, e.Y, (int)espessuraCaneta, (int)espessuraCaneta)); // Transformamos uma variável tipo float em tipo inteiro
                }
            }
        }

        private void buttonLimpar_Click(object sender, EventArgs e)
        {
            // A MessageBox representa uma janela para exibição de mensagens ao usuário
            if(MessageBox.Show("Tem certeza disso? Todo o desenho será apagado!", "Apagar desenho", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                graphicsPainelPintura.Clear(Color.White); // Limpa todo o desenho e preenche novamente o fundo do painel de branco
                imagemASalvar = new Bitmap(panelPintura.Width, panelPintura.Height); // imagem para salvar
                graphicsImagemASalvar = Graphics.FromImage(imagemASalvar); // extraindo graphics da imagem para salvar de forma a podermos desenhar nela
                graphicsImagemASalvar.Clear(panelPintura.BackColor); // Preenchemos a imagem com a cor do fundo do painel
            }
        }

        // SelectedIndexChanged ocorre quando é feita uma escolha de item pelo usuário na comboBox
        private void comboBoxEspessuraDaCaneta_SelectedIndexChanged(object sender, EventArgs e)
        {
            espessuraCaneta = float.Parse(comboBoxEspessuraDaCaneta.SelectedItem.ToString());
        }

        private void buttonBorracha_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) // Se clicar com botão direito do mouse sob o botão borracha
            {
                var colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    corBorracha = colorDialog.Color; // Seleciona a cor da borracha de acordo com a escolha do usuário
                }
            }
            else
            {
                if(!flagApagar) // O operador ! antes irá virar o valor da flagApagar ao contrário, ou seja, se é true ficará false
                {
                    flagApagar = true;
                    buttonBorracha.BackColor = corBorracha; // cor do botão será a mesma da borracha
                }
                else
                {
                    flagApagar = false; // Borracha não estará mais selecionada
                    buttonBorracha.BackColor = Color.Black; // cor do botão voltará ao padrão
                }
            }
        }

        private void buttonSalvar_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog(); // Janela para salvar arquivos
            saveFileDialog.Filter = "Portable Network Graphics|.png|Arquivo JPEG|.jpeg"; // Atribuindo formatos de imagem para o usuário escolher
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Definindo a extensão da imagem que iremos salvar
                switch(saveFileDialog.FilterIndex)
                {
                    case 1:
                        imagemASalvar.Save(saveFileDialog.FileName, ImageFormat.Png);
                        break;
                    case 2:
                        imagemASalvar.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                        break;
                }
            }
        }

        // Evento disparado sempre que o painel é redimensionado
        private void panelPintura_Resize(object sender, EventArgs e)
        {
            // As linhas de código abaixo são necessárias pois ao redimensionar o painel
            // o objeto graphics passa a ter dimensões diferentes, logo, precisamos atualizar 
            // a referência da variável graphicsPainelPintura de forma a podermos desenhar
            // nesse novo espaço.
            // Também precisamos garantir que todo o desenho já feito pelo usuário não seja
            // perdido ao aumentarmos o tamanho do painel de desenho, então precisamos 
            // copiá-lo para a imagem a ser salva. Como não podemos redimensionar um
            // objeto Image depois dele já ter sido instanciado, criamos uma imagem
            // temporária e copiamos para ela tudo aquilo que já foi desenhado anteriormente
            // através do método DrawImage(). Por fim, atualizamos as referências das variáveis
            // imagemASalvar e graphicsImagemASalvar de forma tornarmos a imagem temporária
            // a imagem a ser salva.

            graphicsPainelPintura = panelPintura.CreateGraphics(); // Atualiza a referência do objeto graphics do painel
            var imgTemp = new Bitmap(panelPintura.Width, panelPintura.Height); // Criamos uma imagem temporária
            var graphicsImgTemp = Graphics.FromImage(imgTemp);
            graphicsImgTemp.DrawImage(imagemASalvar, 0, 0); // Desenhamos a imagem antiga na imagem temporária - maneira de fazer uma cópia dos dados
            imagemASalvar = imgTemp;
            graphicsImagemASalvar = graphicsImgTemp;
        }
    }
}
