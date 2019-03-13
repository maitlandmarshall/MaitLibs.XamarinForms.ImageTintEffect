using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ResolutionGroupName("ImageTintEffect")]
[assembly:ExportEffect(typeof(MaitLibs.XamarinForms.ImageTintEffect.Platform.iOS.ImageTintEffect), nameof(MaitLibs.XamarinForms.ImageTintEffect.Platform.iOS.ImageTintEffect))]
namespace MaitLibs.XamarinForms.ImageTintEffect.Platform.iOS
{
    public class ImageTintEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            this.Element.PropertyChanged += this.Element_PropertyChanged;
            this.SetTint();
        }

        protected override void OnDetached()
        {
            this.Element.PropertyChanged -= this.Element_PropertyChanged;
        }

        private void Element_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Shared.ImageTintEffect.TintColor):
                case nameof(Image.Source):
                    this.SetTint();
                    break;
            }
        }

        private void SetTint()
        {
            UIImageView image = this.Control as UIImageView;

            if (image == null)
                return;

            if (image.Image == null)
                return;

            Color newTintColor = Shared.TintEffect.GetTintColor(this.Element);

            image.Image = image.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            image.TintColor = newTintColor.ToUIColor();
        }
    }
}