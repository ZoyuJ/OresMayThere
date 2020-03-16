using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GTOresMayHere {
  public class CusShaderEffect : ShaderEffect {
    public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(CusShaderEffect), 0);
    public Brush Input {
      get => (Brush)GetValue(InputProperty);
      set => SetValue(InputProperty, value);
    }


    // Using a DependencyProperty as the backing store for MixedColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MixedColorProperty = DependencyProperty.Register("MixedColor", typeof(Color), typeof(CusShaderEffect), new UIPropertyMetadata(Colors.Red,PixelShaderConstantCallback(0)));
    public Color MixedColor {
      get { return (Color)GetValue(MixedColorProperty); }
      set { SetValue(MixedColorProperty, value); }
    }



    public CusShaderEffect() {
      PixelShader _PixelShader = new PixelShader();
      _PixelShader.UriSource = new Uri(@"/GTOresMayHere;component/Resources/test1.ps",UriKind.Relative);
      PixelShader = _PixelShader;
      UpdateShaderValue(InputProperty);
      UpdateShaderValue(MixedColorProperty);
    }



  }
}
