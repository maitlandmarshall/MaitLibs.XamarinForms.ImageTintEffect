using CompositionProToolkit;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Effects;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly:ResolutionGroupName("ImageTintEffect")]
[assembly:ExportEffect(typeof(MaitLibs.XamarinForms.ImageTintEffect.Platform.UWP.ImageTintEffect), nameof(MaitLibs.XamarinForms.ImageTintEffect.Platform.UWP.ImageTintEffect))]
namespace MaitLibs.XamarinForms.ImageTintEffect.Platform.UWP
{
    // Thanks to https://github.com/shrutinambiar/xamarin-forms-tinted-image/blob/master/src/Plugin.CrossPlatformTintedImage.UWP/TintedImageRenderer.cs
    // for the UWP code

    public class ImageTintEffect : PlatformEffect
    {
        CompositionEffectBrush effectBrush;
        SpriteVisual spriteVisual;
        IImageSurface imageSurface;
        Compositor compositor;
        ICompositionGenerator generator;

        Color TintColor { get; set; }

        protected override void OnAttached()
        {
            this.Element.PropertyChanged += this.Element_PropertyChanged;
        }

        protected override void OnDetached()
        {
            this.Element.PropertyChanged -= this.Element_PropertyChanged;
        }

        private async void Element_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.TintColor = MaitLibs.XamarinForms.ImageTintEffect.Shared.TintEffect.GetTintColor(this.Element);

            try
            {
                if (this.effectBrush != null)
                {
                    if (this.TintColor == Color.Transparent)
                    {
                        //Turn off tinting - need to redraw brush
                        effectBrush = null;
                        spriteVisual = null;
                    }
                    else
                    {
                        SetTint(GetNativeColor(this.TintColor));
                        return;
                    }
                }

                bool needsResizing = false;
                needsResizing = 
                    e.PropertyName == VisualElement.XProperty.PropertyName ||
                    e.PropertyName == VisualElement.YProperty.PropertyName ||
                    e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                    e.PropertyName == VisualElement.HeightProperty.PropertyName ||
                    e.PropertyName == VisualElement.ScaleProperty.PropertyName ||
                    e.PropertyName == VisualElement.TranslationXProperty.PropertyName ||
                    e.PropertyName == VisualElement.TranslationYProperty.PropertyName ||
                    e.PropertyName == VisualElement.RotationYProperty.PropertyName ||
                    e.PropertyName == VisualElement.RotationXProperty.PropertyName ||
                    e.PropertyName == VisualElement.RotationProperty.PropertyName ||
                    e.PropertyName == VisualElement.AnchorXProperty.PropertyName ||
                    e.PropertyName == VisualElement.AnchorYProperty.PropertyName;

                VisualElement element = this.Element as VisualElement;

                if (spriteVisual != null && imageSurface != null && needsResizing)
                {
                    //Resizing Sprite Visual and Image Surface

                    spriteVisual.Size = new Vector2((float)element.Width, (float)element.Height);
                    imageSurface.Resize(new Windows.Foundation.Size(element.Width, element.Height));

                    return;
                }

                if (e.PropertyName == Xamarin.Forms.Image.SourceProperty.PropertyName || spriteVisual == null || (effectBrush == null && this.TintColor != Color.Transparent))
                    await CreateTintEffectBrushAsync(new Uri($"ms-appx:///{((FileImageSource)(this.Element as Xamarin.Forms.Image).Source).File}"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to create Tinted Image. Exception: {ex.Message}");
            }
        }

        void SetTint(Windows.UI.Color color)
        {
            effectBrush?.Properties.InsertColor("colorSource.Color", color);
        }

        async Task CreateTintEffectBrushAsync(Uri uri)
        {
            Xamarin.Forms.Image element = this.Element as Xamarin.Forms.Image;

            if (Control == null || Element == null || element.Width < 0 || element.Height < 0)
                return;

            SetupCompositor();

            spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Size = new Vector2((float)element.Width, (float)element.Height);

            imageSurface = await generator.CreateImageSurfaceAsync(uri, new Windows.Foundation.Size(element.Width, element.Height), ImageSurfaceOptions.DefaultOptimized);
            CompositionSurfaceBrush surfaceBrush = compositor.CreateSurfaceBrush(imageSurface.Surface);

            CompositionBrush targetBrush = surfaceBrush;

            if (this.TintColor == Color.Transparent)
            {
                // Don't apply tint effect
                effectBrush = null;
            }
            else
            {
                // Set target brush to tint effect brush

                Windows.UI.Color nativeColor = GetNativeColor(this.TintColor);

                IGraphicsEffect graphicsEffect = new CompositeEffect
                {
                    Mode = CanvasComposite.DestinationIn,
                    Sources =
                    {
                        new ColorSourceEffect
                        {
                            Name = "colorSource",
                            Color = nativeColor
                        },
                        new CompositionEffectSourceParameter("mask")
                    }
                };

                CompositionEffectFactory effectFactory = compositor.CreateEffectFactory(graphicsEffect,
                    new[] { "colorSource.Color" });

                effectBrush = effectFactory.CreateBrush();
                effectBrush.SetSourceParameter("mask", surfaceBrush);

                SetTint(nativeColor);

                targetBrush = effectBrush;
            }

            spriteVisual.Brush = targetBrush;
            ElementCompositionPreview.SetElementChildVisual(Control, spriteVisual);
        }

        void SetupCompositor()
        {
            if (compositor != null && generator != null)
                return;

            compositor = ElementCompositionPreview.GetElementVisual(Control).Compositor;
            generator = compositor.CreateCompositionGenerator();
        }

        static Windows.UI.Color GetNativeColor(Color color)
        {
            return Windows.UI.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
        }
    }
}
