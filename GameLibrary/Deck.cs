using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GameCard
{
    public class Deck
    {
        private List<Card> deck = new List<Card>();

        public Deck()
        {
            MakeDeck();
        }

        private void MakeDeck()
        {
            // there are 4 suits
            foreach (int s in Enum.GetValues(typeof(Suit)))
            {
                // there are 13 cards per suit
                foreach (int v in Enum.GetValues(typeof(FaceValue)))
                {
                    // create a card for the current suit and value
                    Card newCard = new Card((Suit)s, (FaceValue)v);

                    // add the card to the deck
                    deck.Add(newCard);
                }
            }
        }

        public void Shuffle()
        {
            Random rGen = new Random();
            List<Card> newDeck = new List<Card>();

            while (deck.Count > 0)
            {
                int removeIndex = rGen.Next(0, (deck.Count));
                Card removeObject = deck[removeIndex];
                deck.RemoveAt(removeIndex);
                //  Add the removed card to the new deck.
                newDeck.Add(removeObject);
            }

            //  replace the old deck with the new deck
            deck = newDeck;
        }

        public Hand DealHand(int number, string namePlayer)
        {
            // check if any cards are left in the deck.  If there are not, throw exception
            if (deck.Count == 0)
            {
                throw new ConstraintException("There are no cards left in the deck.  Redeal");
            }

            // create a new Hand
            Hand hand = new Hand(namePlayer);

            if (deck.Count < number)
            {
                number = deck.Count;
            }

            for (int i = 0; i < number; i++)
            {
                hand.AddCard(DrawOneCard());
            }

            return hand;
        }

        public Card DrawOneCard()
        {
            Card topCard;

            if (deck.Count > 0)
            {
                topCard = deck[0];
                deck.RemoveAt(0);
            }
            else
            {
                throw new ArgumentException("There are no cards in the deck to draw from - deal again");
            }

            return topCard;
        }

        public void DeleteAnyQueenNotSpadesCard()
        {
            var card = deck.FirstOrDefault(d => d.Suit != Suit.Spades && d.FaceValue == FaceValue.Queen);
            deck.Remove(card);
        }

        public List<Hand> DealCards(int countPlayers) //раздать карты
        {
            DeleteAnyQueenNotSpadesCard();//убираем одну даму не пик
            var countCardsForPlayer = deck.Count / countPlayers;//делим карты между игроками
            List<Hand> hands=new List<Hand>();
            String[] players = {"Erica", "Vlada"};

            for (int i = 0; i < countPlayers; i++)
            {
               hands.Add(DealHand(countCardsForPlayer, players[i]));
            }

            while (deck.Count > 0)
            {
               hands[new Random().Next(0,countPlayers-1)].AddCard(deck[0]);
               deck.Remove(deck[0]);
            }
            FoldSameCards(hands);
            return hands;
        }

        void FoldSameCards(List<Hand> hands) //скидывать одинаковые карты
        {
            foreach (var hand in hands)
            {
                RemoveSameCards(hand);
            }
        }

        void RemoveSameCards(Hand hand)
        {
            var cards = hand.GetCards();
            var count =cards.Count;
            var temp=new List<Card>();   

            for (int i = 0; i < count; i++)
            {
                var card = cards.FirstOrDefault(d=> !d.ToString().Equals("QueenSpades"));
                var sameCard = cards.FirstOrDefault(d => card != null && d.FaceValue == card.FaceValue && d.Suit != card.Suit && !d.ToString().Equals("QueenSpades"));
                if (sameCard != null)
                {
                    cards.Remove(card);
                    cards.Remove(sameCard);
                }
                else if(card!=null)
                {
                    cards.Remove(card);
                    temp.Add(card);
                }
                else
                {
                    break;
                }

            }
            temp.ForEach(hand.AddCard);
            hand.Shuffle();
        }


    }
}
