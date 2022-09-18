using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Linq;

namespace CustomNumberBoxSampleApp;

public class CustomNumberBox : NumberBox
{
    public CustomNumberBox()
    {
        Loaded += CustomNumberBox_Loaded;
        LayoutUpdated += CustomNumberBox_LayoutUpdated;
        GotFocus += CustomNumberBox_GotFocus;
        LostFocus += CustomNumberBox_LostFocus;
    }

    private ScrollViewer? ContentElement { get; set; }

    private Thickness ContentElementDefaultMargin { get; set; }

    private RepeatButton? DownSpinButton { get; set; }

    private RepeatButton? UpSpinButton { get; set; }

    private TextBox? InputBox { get; set; }

    private Thickness InputBoxDefaultPadding { get; set; }

    public static IEnumerable<T> FindChildrenOfType<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);

            if (child is T hit)
            {
                yield return hit;
            }

            foreach (T? grandChild in FindChildrenOfType<T>(child))
            {
                yield return grandChild;
            }
        }
    }

    private void CustomNumberBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (ContentElement is not null)
        {
            ContentElement.Margin = ContentElementDefaultMargin;
        }
    }

    private void CustomNumberBox_LayoutUpdated(object? sender, object e)
    {
        if (ContentElement is not null && UpSpinButton is not null && DownSpinButton is not null)
        {
            Thickness margin = ContentElement.Margin;
            ContentElement.Margin = new Thickness(
                margin.Left,
                margin.Top,
                GetRightOffset(),
                margin.Bottom);
            LayoutUpdated -= CustomNumberBox_LayoutUpdated;
        }
    }

    private void CustomNumberBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (FindChildrenOfType<ScrollViewer>(this)
            .Where(x => x.Name == nameof(ContentElement))
            .FirstOrDefault() is ScrollViewer scrollViewer)
        {
            ContentElement = scrollViewer;
            ContentElement.HorizontalAlignment = HorizontalContentAlignment;
            ContentElementDefaultMargin = ContentElement.Margin;
        }

        UpSpinButton = FindChildrenOfType<RepeatButton>(this)
            .Where(x => x.Name == (nameof(UpSpinButton)))
            .FirstOrDefault();

        DownSpinButton = FindChildrenOfType<RepeatButton>(this)
            .Where(x => x.Name == (nameof(DownSpinButton)))
            .FirstOrDefault();

        if (FindChildrenOfType<TextBox>(this)
            .Where(x => x.Name == nameof(InputBox))
            .FirstOrDefault() is TextBox textBox)
        {
            InputBox = textBox;
            InputBoxDefaultPadding = InputBox.Padding;
            InputBox.TextChanged += InputBox_TextChanged;
        }
    }

    private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (textBox.Text.Length > 0)
            {
                textBox.Padding = InputBoxDefaultPadding;
            }
            else
            {
                textBox.Padding = new Thickness(
                    InputBoxDefaultPadding.Left,
                    InputBoxDefaultPadding.Top,
                    GetRightOffset(),
                    InputBoxDefaultPadding.Bottom);
            }
        }
    }

    private void CustomNumberBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (ContentElement is not null)
        {
            ContentElement.Margin = new Thickness(
                ContentElementDefaultMargin.Left,
                ContentElementDefaultMargin.Top,
                GetRightOffset(),
                ContentElementDefaultMargin.Bottom);
        }
    }

    private double GetRightOffset()
    {
        double? rightOffset = 0.0;
        rightOffset += UpSpinButton?.Margin.Left + UpSpinButton?.Margin.Right + UpSpinButton?.ActualWidth;
        rightOffset += DownSpinButton?.Margin.Left + DownSpinButton?.Margin.Right + DownSpinButton?.ActualWidth;
        return rightOffset ?? 0.0;
    }
}