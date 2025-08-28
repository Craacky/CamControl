using System.Windows.Controls;
using System.Windows.Input;

namespace CC.UI.Views.Aggregation;

public partial class AddProductAggregationView
{
    public AddProductAggregationView()
    {
        InitializeComponent();
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
        var textBox = sender as TextBox;
        if (e.Key != Key.Enter) return;
        textBox!.Text += "\0";
    }
}