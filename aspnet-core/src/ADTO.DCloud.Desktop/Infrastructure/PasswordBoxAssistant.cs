using System.Windows;
using System.Windows.Controls;

namespace ADTO.DCloud.Desktop.Infrastructure;

public static class PasswordBoxAssistant
{
    public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
        "BoundPassword",
        typeof(string),
        typeof(PasswordBoxAssistant),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

    private static readonly DependencyProperty UpdatingPasswordProperty = DependencyProperty.RegisterAttached(
        "UpdatingPassword",
        typeof(bool),
        typeof(PasswordBoxAssistant));

    public static string GetBoundPassword(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(BoundPasswordProperty);

    public static void SetBoundPassword(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(BoundPasswordProperty, value);

    private static bool GetUpdatingPassword(DependencyObject dependencyObject) => (bool)dependencyObject.GetValue(UpdatingPasswordProperty);

    private static void SetUpdatingPassword(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(UpdatingPasswordProperty, value);

    private static void OnBoundPasswordChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not PasswordBox passwordBox)
        {
            return;
        }

        passwordBox.PasswordChanged -= HandlePasswordChanged;

        if (!GetUpdatingPassword(passwordBox))
        {
            passwordBox.Password = (string?)e.NewValue ?? string.Empty;
        }

        passwordBox.PasswordChanged += HandlePasswordChanged;
    }

    private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = (PasswordBox)sender;
        SetUpdatingPassword(passwordBox, true);
        SetBoundPassword(passwordBox, passwordBox.Password);
        SetUpdatingPassword(passwordBox, false);
    }
}
