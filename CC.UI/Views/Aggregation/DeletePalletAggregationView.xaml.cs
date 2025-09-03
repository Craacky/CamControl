using System.Windows.Controls;
using System.Windows.Input;

namespace CC.UI.Views.Aggregation;

public partial class DeletePalletAggregationView : UserControl
{
    public DeletePalletAggregationView()
    {
        InitializeComponent();
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
        var textBox = sender as TextBox;
        if (e.Key == Key.Enter) textBox!.Text = "\0";
    }
}