using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Linq;

namespace CustomControls;

public sealed class CustomNumberBox : NumberBox
{
    private ScrollViewer? contentElementScrollViewer;

    private RepeatButton? upSpinButton;

    private RepeatButton? downSpinButton;

    private TextBox? inputTextBox;

    private Thickness contentElementScrollViewerDefaultMargin;

    private Thickness inputTextBoxDefaultPadding;

    public bool IsAZero(int a)
    {
        return a is 0;
    }

    public static IEnumerable<T> FindChildrenOfType<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);

            if (child is T hit)
            {
                yield return hit;
            }

            foreach (var granChild in FindChildrenOfType<T>(child).ToList())
            {
                yield return granChild;
            }
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        this.Loaded += CustomNumberBox_Loaded;
        this.GotFocus += CustomNumberBox_GotFocus;
        this.LostFocus += CustomNumberBox_LostFocus;
    }

    private void CustomNumberBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (FindChildrenOfType<ScrollViewer>(this)
            .Where(x => x.Name == "ContentElement")
            .FirstOrDefault() is ScrollViewer scrollViewer)
        {
            contentElementScrollViewer = scrollViewer;
            contentElementScrollViewer.HorizontalAlignment = HorizontalContentAlignment;
            contentElementScrollViewerDefaultMargin = contentElementScrollViewer.Margin;
        }

        if (FindChildrenOfType<TextBox>(this)
            .Where(x => x.Name == "InputBox")
            .FirstOrDefault() is TextBox textBox)
        {
            inputTextBox = textBox;
            inputTextBoxDefaultPadding = inputTextBox.Padding;
            inputTextBox.TextChanged += InputBox_TextChanged;
        }

        upSpinButton = FindChildrenOfType<RepeatButton>(this)
            .Where(x => x.Name == "UpSpinButton")
            .FirstOrDefault();

        downSpinButton = FindChildrenOfType<RepeatButton>(this)
            .Where(x => x.Name == "DownSpinButton")
            .FirstOrDefault();
    }

    private void CustomNumberBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (contentElementScrollViewer is not null)
        {
            contentElementScrollViewer.Margin = contentElementScrollViewerDefaultMargin;
        }

        foreach (var button in (FindChildrenOfType<Button>(this)))
        {
            button.Foreground = new SolidColorBrush(Colors.HotPink);
            button.FontWeight = FontWeights.Bold;
        }
    }

    private void CustomNumberBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (contentElementScrollViewer is not null && downSpinButton is not null && upSpinButton is not null)
        {
            double rightMargin = downSpinButton.ActualWidth + upSpinButton.ActualWidth;

            contentElementScrollViewer.Margin = new Thickness(
                contentElementScrollViewerDefaultMargin.Left,
                contentElementScrollViewerDefaultMargin.Top,
                rightMargin,
                contentElementScrollViewerDefaultMargin.Bottom);
        }
    }

    private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox && upSpinButton is not null && downSpinButton is not null)
        {
            if (textBox.Text.Length > 0)
            {
                textBox.Padding = inputTextBoxDefaultPadding;
            }
            else
            {
                var thickness = inputTextBoxDefaultPadding;
                thickness.Right += upSpinButton.ActualWidth + downSpinButton.ActualWidth;
                textBox.Padding = thickness;
            }
        }
    }
}
