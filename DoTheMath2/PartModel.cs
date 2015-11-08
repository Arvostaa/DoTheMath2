using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

/*Created by David Tvildiani*/
namespace DoTheMath2
{
    public enum Piece { Free, Head, Body, Food }
    public enum Direction { Up, Down, Left, Right }

    public class BoardPeaceModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Number { get; private set; }
        private BitmapImage _Image;
        public Piece Type { get; private set; }

        public BoardPeaceModel(Piece type, int number)
        {
            ChangeType(type);
            Number = number;
        }

        public void ChangeType(Piece type)
        {
            switch (type)
            {
                case Piece.Free:
                    Image = ImagePeaceType.Free;
                    break;
                case Piece.Head:
                    Image = ImagePeaceType.Head;
                    break;
                case Piece.Body:
                    Image = ImagePeaceType.Body;
                    break;
                case Piece.Food:
                    Image = ImagePeaceType.Food;
                    break;
            }

            Type = type;
        }

        public BitmapImage Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
                NotifyPropertyChanged("Image");
            }
        }

        private void NotifyPropertyChanged(String PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

    }

    /// <summary>
    /// Static class For Loading Images used in game only once :)
    /// </summary>
    public static class ImagePeaceType
    {
        static ImagePeaceType()
        {
            Head = new BitmapImage();
            Head.BeginInit();
            Head.UriSource = new Uri("pack://application:,,,/images/head.png");
            Head.EndInit();


            Body = new BitmapImage();
            Body.BeginInit();
            Body.UriSource = new Uri("pack://application:,,,/images/body.png");
            Body.EndInit();

            Food = new BitmapImage();
            Food.BeginInit();
            Food.UriSource = new Uri("pack://application:,,,/images/food.png");
            Food.EndInit();
        }

        public static BitmapImage Free { get; private set; }
        public static BitmapImage Head { get; private set; }
        public static BitmapImage Body { get; private set; }
        public static BitmapImage Food { get; private set; }
    }
}
