using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TextAnalyseWPF.Command;
using TextAnalyseWPF.Models;

namespace TextAnalyseWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public enum AlgorithmType
        {
            [Description("Расстояние Левенштейна")]
            Levenshtein,
            [Description("N-граммы")]
            NGrams
        }

        private string _textA;
        private string _textB;
        private double _result;
        private bool _ignoreCase;
        private AlgorithmType _selectedAlgorithm = AlgorithmType.Levenshtein;
        private RelayCommand _compareCommand;

        public string TextA
        {
            get { return _textA; }
            set
            {
                _textA = value;
                OnPropertyChanged();
            }
        }

        public string TextB
        {
            get { return _textB; }
            set
            {
                _textB = value;
                OnPropertyChanged();
            }
        }

        public double Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        public string ResultText
        {
            get
            {
                if(string.IsNullOrWhiteSpace(TextA) && string.IsNullOrWhiteSpace(TextB))
                {
                    return "Вставьте текст";
                }
                else if (string.IsNullOrWhiteSpace(TextA))
                {
                    return "Вставьте Текст 1";
                }
                else if (string.IsNullOrWhiteSpace(TextB))
                {
                    return "Вставьте Текст 2";
                }
                else
                {
                    int similarityPercent = (int)(Result * 100);
                    int differentPercent = 100 - similarityPercent;
                    return $"Процент схожести текстов {similarityPercent}%. Разница составляет {differentPercent}%";
                }
            }
        }

        public bool IgnoreCase
        {
            get { return _ignoreCase; }
            set
            {
                _ignoreCase = value;
                OnPropertyChanged();
            }
        }

        public AlgorithmType SelectedAlgorithm
        {
            get
            {
                return _selectedAlgorithm;
            }
            set
            {
                _selectedAlgorithm = value;
                OnPropertyChanged();
            }
        }

        public List<AlgorithmType> Algorithms
        {
            get
            {
                return Enum.GetValues(typeof(AlgorithmType)).Cast<AlgorithmType>().ToList();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            
        }

        public ICommand CompareCommand
        {
            get
            {
                if (_compareCommand == null)
                {
                    _compareCommand = new RelayCommand(Compare);
                }
                return _compareCommand;
            }
        }

        private void Compare()
        {
            var comparer = new TextAnalyseModel();
            switch (SelectedAlgorithm)
            {
                case AlgorithmType.Levenshtein:
                    Result = comparer.CompareTextsUsingLevenshtein(TextA, TextB, IgnoreCase);
                    break;
                case AlgorithmType.NGrams:
                    Result = comparer.CompareTextsUsingNGrams(TextA, TextB, IgnoreCase);
                    break;
                default:
                    break;
            }
            
            OnPropertyChanged(nameof(ResultText));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
