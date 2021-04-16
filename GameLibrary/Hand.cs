using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCard
{
    public class Hand
    {
        private List<Card> cards = new List<Card>();

        public int Count
        {
            get
            {
                return cards.Count();
            }
        }

        public Card this[int index]
        {
            get
            {
                return cards[index];
            }
        }

        public string NameHand { get; set; }

        public Hand()
        {
            
        }

        public Hand(string nameHand)
        {
            NameHand = nameHand;
        }

        public Card this[string faceValue, string suit]
        {
            get { return cards.FirstOrDefault(d => d.ToString().Equals(faceValue + suit)); }
        }

        public void AddCard(Card newCard)
        {
            // the List<T>.Contains method cannot be used since it only checks if the same reference object exists
            if (ContainsCard(newCard))
            {
                throw new ConstraintException(newCard.FaceValue.ToString() + " of " +
                    newCard.Suit.ToString() + " already exists in the Handsss");
            }

            cards.Add(newCard);
        }

        /// <summary>
        /// Remove a card from the hand using a card index
        /// </summary>
        /// <param name="index">The index of the card in the generic list</param>
        public void RemoveCard(int index)
        {
            //5 cards in hand
            //c 1    2   3   4   5
            //i 0    1   2   3   4
            if (index >= 0 && index <= cards.Count - 1)
            {
                //safely remove card from cards list
                cards.RemoveAt(index);
            }
            else
            {
                throw new DataException("Index value exceeds the number of cards in hand");
            }
        }

        /// <summary>
        /// Remove a card by reference
        /// </summary>
        /// <param name="theCard">The card refence to remove</param>
        public void RemoveCard(Card theCard)
        {
            if (ContainsCard(theCard))
            {
                Card findCard = cards.Where(c => c.FaceValue == theCard.FaceValue && c.Suit == theCard.Suit).FirstOrDefault();
                cards.Remove(findCard);
            }
            else
            {
                throw new DataException($"The card {theCard.Suit} of {theCard.FaceValue} does not exist in the hand");
            }
        }

        /// <summary>
        /// Remove card by providing suit and face value
        /// </summary>
        /// <param name="theSuit">The card suit to remove</param>
        /// <param name="theValue">The card face value to remove</param>
        public void RemoveCard(Suit theSuit, FaceValue theValue)
        {
            //DRY
            Card c = new Card(theSuit, theValue);
            RemoveCard(c);
        }

        /// <summary>
        /// Checks to see if a card of suit and face is in the hand of cards
        /// </summary>
        /// <param name="cardToCheck"></param>
        /// <returns></returns>
        public bool ContainsCard(Card cardToCheck)
        {
            foreach (Card card in cards)
            {
                if (card.FaceValue == cardToCheck.FaceValue && card.Suit == cardToCheck.Suit)
                {
                    return true;
                }
            }

            return false;
        }

        public List<Card> GetCards()
        {
            return cards;
        }

        public bool IsQeenSpades(Card card)
        {
            return card.FaceValue == FaceValue.Queen && card.Suit == Suit.Spades;
        }


        public Card GetSameCard(Card card)
        {
            foreach (var card1 in cards)
            {
                if (card1.FaceValue == card.FaceValue)
                {
                    if (card1.Suit == Suit.Spades && card1.FaceValue == FaceValue.Queen)
                    {
                        continue;
                    }
                    else
                    {
                        return card1;
                    }
                }
            }

            return null;
            //var find= cards.FirstOrDefault(d => d.FaceValue == card.FaceValue 
            //                                    && (d.FaceValue != FaceValue.Queen && d.Suit != Suit.Spades) );
            //var res = find != null && find.FaceValue == FaceValue.Queen && find.Suit == Suit.Spades ? null : find;
            //return res;
        }

        public void Shuffle()
        {
            Random rGen = new Random();
            List<Card> newDeck = new List<Card>();

            while (cards.Count > 0)
            {
                int removeIndex = rGen.Next(0, (cards.Count));
                Card removeObject = cards[removeIndex];
                cards.RemoveAt(removeIndex);
                //  Add the removed card to the new deck.
                newDeck.Add(removeObject);
            }

            //  replace the old deck with the new deck
            cards = newDeck;
        }
    }

}

