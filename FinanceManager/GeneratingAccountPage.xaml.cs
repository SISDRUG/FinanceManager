using Microsoft.Maui.Controls;

namespace FinanceManager;

public partial class GeneratingAccountPage : ContentPage
{
    public GeneratingAccountPage()
    {
        InitializeComponent();
        CarouselTest.SetBinding(ItemsView.ItemsSourceProperty, "Monkeys");

        CarouselTest.ItemTemplate = new DataTemplate(() =>
        {
            Label nameLabel = new Label { Text= "1"};
            nameLabel.SetBinding(Label.TextProperty, "Name");

            Image image = new Image { Source = "fasupport.png" };
            image.SetBinding(Image.SourceProperty, "ImageUrl");

            Label locationLabel = new Label { Text = "2" };
            locationLabel.SetBinding(Label.TextProperty, "Location");

            Label detailsLabel = new Label { Text = "3" };
            detailsLabel.SetBinding(Label.TextProperty, "Details");

            StackLayout stackLayout = new StackLayout
            {
                nameLabel,
                image,
                locationLabel,
                detailsLabel
            };

            Frame frame = new Frame { BorderColor = Colors.Gold };
            StackLayout rootStackLayout = new StackLayout
            {
                frame
            };

            return rootStackLayout;
        });
    }
}
