using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MaitLibs.XamarinForms.ImageTintEffect.Shared
{
    public static class TintEffect
    {
        public static BindableProperty TintColorProperty = 
            BindableProperty.CreateAttached(nameof(ImageTintEffect.TintColor), typeof(Color), typeof(TintEffect), Color.Black, propertyChanged: OnTintColorChanged);

        public static Color GetTintColor (BindableObject view)
        {
            return (Color)view.GetValue(TintColorProperty);
        }

        public static void SetTintColor (BindableObject view, Color value)
        {
            view.SetValue(TintColorProperty, value);
        }

        private static void OnTintColorChanged (BindableObject view, object oldValue, object newValue)
        {
            Image image = view as Image;

            if (image == null)
                throw new NotSupportedException($"{nameof(TintEffect)} can only be used with {nameof(Image)}");

            ImageTintEffect effect = image.Effects.FirstOrDefault(y => y is ImageTintEffect) as ImageTintEffect;

            if (effect == null)
            {
                effect = new ImageTintEffect();
                image.Effects.Add(effect);
            }

            effect.TintColor = (Color)newValue;
        }
    }

    internal class ImageTintEffect : RoutingEffect
    {
        public Color TintColor { get; set; }

        protected override void OnAttached()
        {
            
        }

        protected override void OnDetached()
        {
            
        }

        public ImageTintEffect() : base ("ImageTintEffect.ImageTintEffect")
        {

        }
    }
}
