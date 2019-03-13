using Android.Widget;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("ImageTintEffect")]
[assembly: ExportEffect(typeof(MaitLibs.XamarinForms.ImageTintEffect.Platform.Android.ImageTintEffect), nameof(MaitLibs.XamarinForms.ImageTintEffect.Platform.Android.ImageTintEffect))]
namespace MaitLibs.XamarinForms.ImageTintEffect.Platform.Android
{
    public class ImageTintEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            this.Element.PropertyChanged += this.Element_PropertyChanged;
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
            ImageView image = this.Control as ImageView;

            if (image == null)
                return;

            Color newColor = Shared.TintEffect.GetTintColor(this.Element);

            image.SetColorFilter(newColor.ToAndroid());
        }
    }
}