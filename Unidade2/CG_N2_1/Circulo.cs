using CG_Biblioteca;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public Circulo(Objeto paiRef, ref char _rotulo, double raio ,Ponto4D ponto) : base(paiRef)
        {
          List<Ponto4D> pontos = new List<Ponto4D>();
            for (int i = 0; i < 72; i++)
            {
                pontos.Add(Matematica.GerarPtosCirculo(0.5 * i, raio));
            }

            base.pontosLista = pontos;
           Atualizar();
        }

        public void Atualizar()
        {

            base.ObjetoAtualizar();
        }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Ponto _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

    }
}