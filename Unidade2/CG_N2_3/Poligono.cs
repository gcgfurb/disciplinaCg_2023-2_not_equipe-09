#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace gcgcg
{
  internal class Poligono : Objeto
  {
    public Poligono(Objeto paiRef, ref char _rotulo, List<Ponto4D> pontosPoligono) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = 1;
      base.pontosLista = pontosPoligono;
      Atualizar();
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Poligono _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
