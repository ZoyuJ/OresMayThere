sampler2D input: register(s0);
float4 base: register(c0);

float ColorPassBlend(float cp1, float cp2, float ca1, float ca2) {
  return(cp1 * ca1 * (1.0f - ca2) + cp2 * ca2) / (ca1 + ca2 - ca1 * ca2);
}

float4 main(float2 uv:TEXCOORD) :COLOR{
  float4 scolor = tex2D(input,uv);
  float4 base01 = float4(base.r / 255.0f, base.g / 255.0f, base.b / 255.0f, base.a / 255.0f);
  float balpha = base01.a + scolor.a - base01.a * scolor.a;
  return float4(ColorPassBlend(scolor.r, base01.r,scolor.a, base01.a), ColorPassBlend(scolor.g, base01.g, scolor.a, base01.a), ColorPassBlend(scolor.b, base01.b, scolor.a, base01.a), balpha);
}