using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using GameCard;
using Timer = System.Timers.Timer;

namespace GameCardWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Erica начинает ход
        /// </summary>
        public bool IsTurnErica
        {
            get => _isTurnErica;
            set
            {
                _isTurnErica = value;
            }
        }

        private string _whoIsTurn;
        private bool _isTurnErica;
        private bool EricaAnswer;
        private bool VladaAnswer;
        private bool IsTurnVlada;
        List<Card> CardsInDeck = new List<Card>(); //карты на столе
        
        private int numberGame = 0;
        List<Hand> hands = new List<Hand>();

        Image CreateImage(string facevalue, string suit, string nameHand = "")
        {
            Image finalImage = new Image
            {
                Width = 71,
                Height = 100,
                Margin = new Thickness(10),
                Uid = $"{facevalue}|{suit}",
                Cursor = Cursors.Hand
            };
            finalImage.MouseLeftButtonUp += FinalImage_MouseLeftButtonUp;
            var uri = $"pack://application:,,,/GameCardWpf;component/images/{facevalue}{suit}.jpg";
            if (nameHand.Equals("Erica"))//&& !Debugger.IsAttached
            {
                uri = $"pack://application:,,,/GameCardWpf;component/images/cardback.gif";
            }
            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(uri);
            logo.EndInit();

            finalImage.Source = logo;
            return finalImage;
        }

        private void FinalImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClickImage(sender);
        }

        private async void ClickImage(object sender)
        {
            var img = sender as Image;
            var wp = VisualTreeHelper.GetParent(img);
            var sp = VisualTreeHelper.GetParent(wp);
            var spParent = sp as StackPanel;
            Hand hand;
            var split = img.Uid.Split('|');
            var facevalueStr = split[0];
            var suitStr = split[1];
            var facevalue = (FaceValue)Enum.Parse(typeof(FaceValue), facevalueStr);
            var suit = (Suit)Enum.Parse(typeof(Suit), suitStr);
            Card selectedCard = null;
            if (spParent.Name.Equals("EricaPanel") && IsTurnVlada && !EricaAnswer && VladaAnswer == false)
            {
                selectedCard = hands[0][facevalueStr, suitStr];
                hand = hands[0];
                VladaAnswer = true;
            }
            else if (spParent.Name.Equals("VladaPanel") && IsTurnErica && !VladaAnswer)
            {
                selectedCard = hands[1][facevalueStr, suitStr];
                hand = hands[1];
                EricaAnswer = true;
            }
            else if (spParent.Name.Equals("VladaPanel") && VladaAnswer) //если карту отдает Влада то она должна отдавать только равные по величине карты
            {
                selectedCard = hands[1][facevalueStr, suitStr];
                if (CheckCorrectFoldCard(selectedCard))
                {
                    hand = hands[1];
                    IsTurnVlada = false;
                    IsTurnErica = false;
                }
                else
                {
                    MessageBox.Show("Choose correct facevalue!");
                    return;
                }
            }
            else if (spParent.Name.Equals("EricaPanel") && EricaAnswer)
            {
                hand = hands[0];
                selectedCard = hands[0][facevalueStr, suitStr];
            }
            else
            {
                return;
            }

            hand.RemoveCard(suit, facevalue); //удаляем карту у игрока
            ShowHand(hand, spParent); //обновляем карты игрока
            ShowCard(spMiddle, facevalueStr, suitStr, selectedCard); //показываем на столе
            if (VladaAnswer && !IsTurnVlada)
            {
                await Task.Delay(2000);
                RemoveCardFromDeck();
                PassTurn();
            }
            else
            {
                CheckQueenSpades(hand, selectedCard); //если дама пик,забираем себе
            }
        }

        private async void CheckHaveAnswer(Card card)
        {
            var handAnswer = VladaAnswer ? hands[1] : hands[0];
            var answerPanel = VladaAnswer ? VladaPanel : EricaPanel;
            var sameCard = handAnswer.GetSameCard(card);

            if (sameCard == null)
            {
                await Task.Delay(2000);
                RemoveCardFromDeck();
                await Task.Delay(2000);
                TakeCardFromDeck(handAnswer, answerPanel, card);
                await Task.Delay(2000);
                PassTurn();
            }
            else
            {
                await Task.Delay(1000);
                handAnswer.RemoveCard(sameCard);
                DeleteCardFromAnswerPanel(answerPanel, sameCard);
                await Task.Delay(500);
                ShowCard(spMiddle, sameCard.FaceValue.ToString(), sameCard.Suit.ToString(), sameCard);
                await Task.Delay(1000);
                RemoveCardFromDeck();
                await Task.Delay(1000);
                PassTurn();
            }

        }

        void DeleteCardFromAnswerPanel(StackPanel stackPanel, Card card)
        {
            var wrapPanel = stackPanel.Children[0] as WrapPanel;
            var uielements = wrapPanel.Children.Cast<Image>().ToList();
            var image = uielements.FirstOrDefault(d => d.Uid.Replace("|", "") == card.ToString());
            wrapPanel.Children.Remove(image);
        }

        /// <summary>
        /// Передача хода
        /// </summary>
        async void PassTurn()
        {
            var witch = CheckEndGame();
            await Task.Delay(1000);
            if (witch != null)
            {
                AddSpMiddleText($"{witch.NameHand} is the witch!");
                await Task.Delay(5000);
                AddSpMiddleText($"Game over!");
                await Task.Delay(5000);
                GameAgainShowBtn(witch.NameHand);
                return;
            }

            if (VladaAnswer)
            {
                IsTurnErica = true;
                IsTurnVlada = false;
                VladaAnswer = false;
                EricaAnswer = false;
                await Task.Delay(1000);
                AddSpMiddleText($"{WhoIsTurn} Turn's");
                await Task.Delay(1000);
                TurnErica();
                return;
            }
            else
            {
                IsTurnVlada = true;
                IsTurnErica = false;
                EricaAnswer = false;
            }

            await Task.Delay(1000);
            AddSpMiddleText($"{WhoIsTurn} Turn's");
        }

        void GameAgainShowBtn(string looser)
        {
            ClearAfterGameOver();
            if (looser.Equals("Erica"))
            {
                vladaScore++;
            }
            else
            {
                ericaScore++;
            }
            SetScore();
            Button btn = new Button()
            {
                Content = "Play Game",
                FontSize = 55,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            btn.Click += DealCards_Click;
            spMiddle.Children.Add(btn);
        }

        void ClearAfterGameOver()
        {
            CardsInDeck.Clear();
            VladaPanel.Children.Clear();
            EricaPanel.Children.Clear();
            spMiddle.Children.Clear();
            EricaAnswer = false;
            VladaAnswer = false;
            IsTurnVlada = false;
            IsTurnErica = false;
            hands.Clear();
        }



        /// <summary>
        /// Проверка конца игры
        /// </summary>
        /// <param name="passHandFrom"></param>
        /// <param name="toHand"></param>
        /// <returns></returns>
        Hand CheckEndGame()
        {
            var dampik = new Card(Suit.Spades, FaceValue.Queen);
            var handHaveQueenSpades = hands.Where(d => d.ContainsCard(dampik) && d.Count == 1);
            var haveQueenSpades = handHaveQueenSpades.ToList();
            if (haveQueenSpades.Any())
            {
                return haveQueenSpades.FirstOrDefault();
            }

            return null;
        }


        private async void CheckQueenSpades(Hand hand, Card selectedCard)
        {
            await Task.Delay(2000);
            var handAnswer = VladaAnswer ? hands[1] : hands[0];
            var sameCard = handAnswer.GetSameCard(selectedCard);
            if (hand.IsQeenSpades(selectedCard))
            {
                var aHandTarget = IsTurnVlada ? hands[1] : hands[0];
                var sPanel = IsTurnVlada ? VladaPanel : EricaPanel;
                RemoveCardFromDeck();
                TakeCardFromDeck(aHandTarget, sPanel, selectedCard);
                IsTurnErica = !IsTurnErica;
                IsTurnVlada = !IsTurnVlada;
                AddSpMiddleText($"{WhoIsTurn} Turn's");
                PassTurn();
            }
            else if (EricaAnswer )//|| (sameCard == null && VladaAnswer))
            {
                CheckHaveAnswer(selectedCard); //если не дама пик ищем у себя карту номиналом на столе
            }
        }



        void TakeCardFromDeck(Hand handTarget, StackPanel sPanel, Card card)
        {
            handTarget.AddCard(card);
            handTarget.Shuffle();
            ShowHand(handTarget, sPanel);
        }


        bool CheckCorrectFoldCard(Card selectedCard)
        {
            if (selectedCard == null)
                throw new Exception("Selected must be choose!");
            var facevalue = selectedCard.FaceValue;
            var cardInDesk = CardsInDeck[0].FaceValue;
            return facevalue == cardInDesk && !selectedCard.ToString().Equals("QueenSpades");
        }



        void ShowCard(StackPanel sPanel, string facevalue, string suit, Card card)
        {
            if (sPanel.Children.Count == 1 && sPanel.Children[0] is TextBlock)
            {
                sPanel.Children.Clear();
            }


            var img = CreateImage(facevalue, suit);
            sPanel.Children.Add(img);
            CardsInDeck.Add(card);
        }

        /// <summary>
        /// Удалить со стола карты
        /// </summary>
        async void RemoveCardFromDeck()
        {

            //DoubleAnimation da = new DoubleAnimation
            //{
            //    From = 1,
            //    To = 0,
            //    Duration = new Duration(TimeSpan.FromSeconds(2)),
            //    AutoReverse = false
            //};
            //if (spMiddle.Children.Count > 0)
            //    spMiddle.Children[0]?.BeginAnimation(OpacityProperty, da);
            //if (spMiddle.Children.Count > 1)
            //    spMiddle.Children[1]?.BeginAnimation(OpacityProperty, da);
            //await Task.Delay(2000);
            spMiddle.Children.Clear();
            CardsInDeck.Clear();
        }

        private Deck aDeck;
        private int vladaScore = 0;
        private int ericaScore = 0;

        private void SetUp()
        {
            try
            {
                aDeck = new Deck();
                aDeck.Shuffle();
                SetScore();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void SetScore()
        {
            txbScore.Text = $"Vlada {vladaScore}/{ericaScore} Erica";
        }


        private void ShowHand(Hand aHand, StackPanel panel = null)
        {

            WrapPanel wrapPanel = new WrapPanel();
            for (int i = 0; i < aHand.Count; i++)
            {
                var aCard = aHand[i];
                var image = CreateImage(aCard.FaceValue.ToString(), aCard.Suit.ToString(), aHand.NameHand);
                wrapPanel.Children.Add(image);
            }

            if (EricaPanel.Children.Count == 0)
            {
                EricaPanel.Children.Add(wrapPanel);
            }
            else if (VladaPanel.Children.Count == 0)
            {
                VladaPanel.Children.Add(wrapPanel);
            }
            else if (panel != null)
            {
                panel.Children.Clear();
                panel.Children.Add(wrapPanel);
            }
        }




        private void DealCards_Click(object sender, RoutedEventArgs e)
        {
            AddSpMiddleText($"Game {++numberGame}");
            SetUp();
            Run();
            AddSpMiddleText($"{WhoIsTurn}'s Turn");
            if (IsTurnErica)
            {
                TurnErica();
            }
        }

        public string WhoIsTurn
        {
            get => IsTurnErica ? "Erica" : "Vlada";
        }


        async void TurnErica()
        {
            await Task.Delay(2000);
            var wrapPanel = VladaPanel.Children[0] as WrapPanel;
            var uielements = wrapPanel.Children.Cast<Image>().ToList();
            var randomImageIndex = new Random().Next(0, uielements.Count - 1);
            await Task.Delay(2000);
            ClickImage(uielements[randomImageIndex]);
        }


        private void Run()
        {
            hands = aDeck.DealCards(2);
            foreach (var hand in hands)
            {
                ShowHand(hand);
            }

            spMiddle.Children.Clear();
            if (hands[0].Count < hands[1].Count)
            {
                IsTurnErica = true;
            }
            else
            {
                IsTurnVlada = true;
            }
        }


        void AddSpMiddleText(string text)
        {
            spMiddle.Children.Clear();
            TextBlock txBlock = new TextBlock
            {
                Text = text,
                FontSize = 55,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            spMiddle.Children.Add(txBlock);

        }
    }
}
