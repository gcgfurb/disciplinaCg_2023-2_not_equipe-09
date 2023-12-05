#define CG_Gizmo // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using System;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using MatrixMode = OpenTK.Graphics.OpenGL.MatrixMode;

//FIXME: padrão Singleton

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        Objeto mundo;
        private char rotuloNovo = '?';
        private Objeto objetoSelecionado = null;

        private readonly float[] _sruEixos =
        {
            -0.5f, 0.0f, 0.0f, /* X- */ 0.5f, 0.0f, 0.0f, /* X+ */
            0.0f, -0.5f, 0.0f, /* Y- */ 0.0f, 0.5f, 0.0f, /* Y+ */
            0.0f, 0.0f, -0.5f, /* Z- */ 0.0f, 0.0f, 0.5f /* Z+ */
        };

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private int _elementBufferObject;
        private int _vertexBufferObject;
        private int _vertexArrayObject;

        private int _elementBufferObject2;
        private int _vertexBufferObject2;
        private int _vertexArrayObject2;

        private readonly float[] _vertices =
        {
            -1, -1, +1, 0, 0,
            +1, -1, +1, 1, 0,
            +1, +1, +1, 1, 1,
            -1, +1, +1, 0, 1
        };

        private readonly float[] _vertices2 =
        {
            +1, -1, +1, 0, 0,
            +1, -1, +1, 1, 0,
            +1, +1, -1, 1, 1,
            +1, +1, +1, 0, 1
        };


        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private Shader _shaderBranca;
        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;
        private Shader _shaderCiano;
        private Shader _shaderMagenta;
        private Shader _shaderAmarela;
        private Shader _shader;

        private Texture _texture;
        private Texture _texture2;

        private Camera _camera;

        private Cubo cuboMaior;
        private Cubo cuboMenor;

        private readonly Vector3 _lightPos = new Vector3(0, 0, 2f);
        private int _vaoModel;
        private int _vaoLamp;
        private Shader _lampShader;
        private Shader _lightingShader;
        private Vector2 _lastPos;

        private bool ativaLuz = false;

        private Vector2 _lastMousePos;
        
        private bool _firstMove = true;

        private float mouseSensitivity = 0.2f; // Ajuste esse valor conforme necessário


        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _camera = new Camera(Vector3.UnitZ * 5, Size.X / (float)Size.Y);
            mundo = new Objeto(null, ref rotuloNovo);
            cuboMaior = new Cubo(mundo, ref rotuloNovo, new Ponto4D[]
            {
                new Ponto4D(-1.0f, -1.0f, 1.0f),
                new Ponto4D(1.0f, -1.0f, 1.0f),
                new Ponto4D(1.0f, 1.0f, 1.0f),
                new Ponto4D(-1.0f, 1.0f, 1.0f),
                new Ponto4D(-1.0f, -1.0f, -1.0f),
                new Ponto4D(1.0f, -1.0f, -1.0f),
                new Ponto4D(1.0f, 1.0f, -1.0f),
                new Ponto4D(-1.0f, 1.0f, -1.0f)
            });

            cuboMenor = new Cubo(cuboMaior, ref rotuloNovo, new Ponto4D[]
            {
    
                new Ponto4D(2.0f, -0.5f, 1.0f),
                new Ponto4D(3.0f, -0.5f, 1.0f),
                new Ponto4D(3.0f, 0.5f, 1.0f),
                new Ponto4D(2.0f, 0.5f, 1.0f),
                new Ponto4D(2.0f, -0.5f, 0.0f),
                new Ponto4D(3.0f, -0.5f, 0.0f),
                new Ponto4D(3.0f, 0.5f, 0.0f),
                new Ponto4D(2.0f, 0.5f, 0.0f)
                
            });
        }

        private void Diretivas()
        {
#if DEBUG
            Console.WriteLine("Debug version");
#endif
#if RELEASE
    Console.WriteLine("Release version");
#endif
#if CG_Gizmo
            Console.WriteLine("#define CG_Gizmo  // debugar gráfico.");
#endif
#if CG_OpenGL
            Console.WriteLine("#define CG_OpenGL // render OpenGL.");
#endif
#if CG_DirectX
      Console.WriteLine("#define CG_DirectX // render DirectX.");
#endif
#if CG_Privado
      Console.WriteLine("#define CG_Privado // código do professor.");
#endif
            Console.WriteLine("__________________________________ \n");
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Diretivas();

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            GL.Enable(EnableCap.DepthTest); // Ativar teste de profundidade
            GL.Enable(EnableCap.CullFace); // Desenha os dois lados da face
            // GL.FrontFace(FrontFaceDirection.Cw);
            // GL.CullFace(CullFaceMode.FrontAndBack);

            #region Cores

            _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");

            #endregion

            #region Eixos: SRU

            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos,
                BufferUsageHint.StaticDraw);
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            #endregion

            //Copia dos exemplos de shaders

            _lightingShader = new Shader("Shaders/lighting.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/lighting.vert", "Shaders/shader.frag");

            {
                _vaoModel = GL.GenVertexArray();
                GL.BindVertexArray(_vaoModel);

                var positionLocation = _lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                // Remember to change the stride as we now have 6 floats per vertex
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                // We now need to define the layout of the normal so the shader can use it
                var normalLocation = _lightingShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
                    3 * sizeof(float));
            }

            {
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                var positionLocation = _lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                // Also change the stride here as we now have 6 floats per vertex. Now we don't define the normal for the lamp VAO
                // this is because it isn't used, it might seem like a waste to use the same VBO if they dont have the same data
                // The two cubes still use the same position, and since the position is already in the graphics memory it is actually
                // better to do it this way. Look through the web version for a much better understanding of this.
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            }

    

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
                BufferUsageHint.StaticDraw);

            _shader = new Shader("Shaders/lighting.vert", "Shaders/lighting.frag");
            _shader.Use();

            var vertexLocation = _shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));

            var normalLocation2 = _lightingShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation2);
            GL.VertexAttribPointer(normalLocation2, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
                3 * sizeof(float));

            _vertexArrayObject2 = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject2);

            _vertexBufferObject2 = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject2);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices2.Length * sizeof(float), _vertices2,
                BufferUsageHint.StaticDraw);

            _elementBufferObject2 = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject2);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
                BufferUsageHint.StaticDraw);

            _shader.Use();

            var texCoordLocation2 = _shader.GetAttribLocation("aTexCoord2");
            GL.EnableVertexAttribArray(texCoordLocation2);
            GL.VertexAttribPointer(texCoordLocation2, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));


            _texture = Texture.LoadFromFile("D:\\furb\\comp-grafica\\CG_Noturno_2023_2\\Unidade4\\CG_N4_Exemplo\\Imagens\\victor.jpeg");
            _texture.Use(TextureUnit.Texture0);

            /*_texture2 = Texture.LoadFromFile(  "D:\\furb\\comp-grafica\\CG_Noturno_2023_2\\Unidade4\\CG_N4_Exemplo\\Imagens\\leonardo.jpeg");
            _texture2.Use(TextureUnit.Texture1);
            */

            _shader.SetInt("texture0", 0);
            //_shader.SetInt("texture1", 1);

            #region Objeto: Cubo

            objetoSelecionado = cuboMaior;

            #endregion

            _camera = new Camera(Vector3.UnitZ * 5, Size.X / (float)Size.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Atualizando as matrizes de visualização e projeção da câmera
            Matrix4 viewMatrix = _camera.GetViewMatrix();
            Matrix4 projectionMatrix = _camera.GetProjectionMatrix();

            // Configuração e renderização da luz, se ativa
            if (ativaLuz)
            {
                GL.BindVertexArray(_vaoModel);
                _lightingShader.Use();

                _lightingShader.SetMatrix4("model", Matrix4.Identity);
                _lightingShader.SetMatrix4("view", viewMatrix);
                _lightingShader.SetMatrix4("projection", projectionMatrix);

                _lightingShader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
                _lightingShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
                _lightingShader.SetVector3("lightPos", _lightPos);
                _lightingShader.SetVector3("viewPos", _camera.Position);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

                // Configuração e renderização da lâmpada
                GL.BindVertexArray(_vaoLamp);
                _lampShader.Use();

                Matrix4 lampMatrix = Matrix4.CreateScale(0.2f) * Matrix4.CreateTranslation(_lightPos);
                _lampShader.SetMatrix4("model", lampMatrix);
                _lampShader.SetMatrix4("view", viewMatrix);
                _lampShader.SetMatrix4("projection", projectionMatrix);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            // Configuração e renderização do objeto principal
            GL.BindVertexArray(_vertexArrayObject);
            _texture.Use(TextureUnit.Texture0);

            if (ativaLuz)
            {
                _shader.SetVector3("objectColor", new Vector3(1.0f, 1.0f, 1f));
                _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
                _shader.SetVector3("lightPos", _lightPos);
                _shader.SetVector3("viewPos", _camera.Position);
            }

            _shader.Use();
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            // para renderizar um segundo objeto, descomente estas linhas
            // GL.BindVertexArray(_vertexArrayObject2);
            // _texture2.Use(TextureUnit.Texture1);
            // _shader.Use();
            // GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            objetoSelecionado.shaderCor = _shader;
            cuboMenor.MatrizRotacao(0.02);

            mundo.Desenhar(new Transformacao4D(), _camera);

            #if CG_Gizmo
            Gizmo_Sru3D();
            #endif

            SwapBuffers();
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // Primeiro declare a variável mouseState
            var mouseState = MouseState;

            var viewMatrix = _camera.GetViewMatrix();
            var projectionMatrix = _camera.GetProjectionMatrix();

            if (!_firstMove)
            {
                var deltaX = mouseState.X - _lastMousePos.X;
                var deltaY = mouseState.Y - _lastMousePos.Y;
                _lastMousePos = new Vector2(mouseState.X, mouseState.Y);

                _camera.Yaw += deltaX * mouseSensitivity;
                _camera.Pitch -= deltaY * mouseSensitivity;

                _camera.UpdateCameraVectors();
            }
            #region Teclado

            var input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
                Close();
            if (input.IsKeyPressed(Keys.Space))
            {
                if (objetoSelecionado == null)
                    objetoSelecionado = mundo;
                objetoSelecionado.shaderCor = _shaderBranca;
                objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
                objetoSelecionado.shaderCor = _shaderAmarela;
            }

            if (input.IsKeyPressed(Keys.G))
                mundo.GrafocenaImprimir("");
            if (input.IsKeyPressed(Keys.P) && objetoSelecionado != null)
                Console.WriteLine(objetoSelecionado.ToString());
            if (input.IsKeyPressed(Keys.M) && objetoSelecionado != null)
                objetoSelecionado.MatrizImprimir();
            if (input.IsKeyPressed(Keys.I) && objetoSelecionado != null)
                objetoSelecionado.MatrizAtribuirIdentidade();
            if (input.IsKeyDown(Keys.Left) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(-0.005, 0, 0);
            if (input.IsKeyDown(Keys.Right) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0.005, 0, 0);
            if (input.IsKeyDown(Keys.Up) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, 0.005, 0);
            if (input.IsKeyDown(Keys.Down) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, -0.005, 0);
            if (input.IsKeyPressed(Keys.O) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, 0, 0.05);
            if (input.IsKeyPressed(Keys.L) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, 0, -0.05);
            if (input.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
            if (input.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
            if (input.IsKeyPressed(Keys.Home) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
            if (input.IsKeyPressed(Keys.End) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);

            if (input.IsKeyPressed(Keys.D1))
            {
                ativaLuz = !ativaLuz;
            }

            if (input.IsKeyPressed(Keys.D2))
            {
                //4-LightingMaps
            }

            if (input.IsKeyPressed(Keys.D3))
            {
                //5-LightCasters-DirectionalLights
            }

            if (input.IsKeyPressed(Keys.D4))
            {
                //5-LightCasters-PointLights
            }

            if (input.IsKeyPressed(Keys.D5))
            {
                //5-LightCasters-Spotlight
            }

            if (input.IsKeyPressed(Keys.D6))
            {
                //6-MultipleLights
            }

            if (input.IsKeyPressed(Keys.D0))
            {
                //sem iluminação
            }

            const float cameraSpeed = 1.5f;
            if (input.IsKeyDown(Keys.Z))
                _camera.Position = Vector3.UnitZ * 5;
            if (input.IsKeyDown(Keys.W))
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            if (input.IsKeyDown(Keys.S))
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            if (input.IsKeyDown(Keys.A))
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            if (input.IsKeyDown(Keys.D))
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            if (input.IsKeyDown(Keys.RightShift))
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            if (input.IsKeyDown(Keys.LeftShift))
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            if (input.IsKeyDown(Keys.D9))
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            if (input.IsKeyDown(Keys.D0))
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down

            #endregion

            #region Mouse

            if (MouseState.IsButtonPressed(MouseButton.Left))
            {
                Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Left)");
                Console.WriteLine("__ Valores do Espaço de Tela");
                Console.WriteLine("Vector2 mousePosition: " + MousePosition);
                Console.WriteLine("Vector2i windowSize: " + Size);
            }

            if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
            {
                Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");

                int janelaLargura = Size.X;
                int janelaAltura = Size.Y;
                Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
                Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

                objetoSelecionado.PontosAlterar(sruPonto, 0);
            }

            if (MouseState.IsButtonReleased(MouseButton.Right))
            {
                Console.WriteLine("MouseState.IsButtonReleased(MouseButton.Right)");
            }

            #endregion

            if (_firstMove)
            {
                _lastMousePos = new Vector2(mouseState.X, mouseState.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouseState.X - _lastMousePos.X;
                var deltaY = mouseState.Y - _lastMousePos.Y;
                _lastMousePos = new Vector2(mouseState.X, mouseState.Y);

                _camera.Yaw += deltaX * mouseSensitivity;
                _camera.Pitch -= deltaY * mouseSensitivity;

                _camera.UpdateCameraVectors(); // Atualiza os vetores da câmera com base no Pitch e Yaw
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Size.X / (float)Size.Y,
                1.0f, 50.0f);
            OpenTK.Graphics.OpenGL.GL.MatrixMode(MatrixMode.Projection);
            OpenTK.Graphics.OpenGL.GL.LoadMatrix(ref projection);
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

            GL.DeleteProgram(_shaderBranca.Handle);
            GL.DeleteProgram(_shaderVermelha.Handle);
            GL.DeleteProgram(_shaderVerde.Handle);
            GL.DeleteProgram(_shaderAzul.Handle);
            GL.DeleteProgram(_shaderCiano.Handle);
            GL.DeleteProgram(_shaderMagenta.Handle);
            GL.DeleteProgram(_shaderAmarela.Handle);
            GL.DeleteProgram(_shader.Handle);

            base.OnUnload();
        }

#if CG_Gizmo
        private void Gizmo_Sru3D()
        {
#if CG_OpenGL && !CG_DirectX
            var model = Matrix4.Identity;
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // EixoX
            _shaderVermelha.SetMatrix4("model", model);
            _shaderVermelha.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVermelha.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.SetMatrix4("model", model);
            _shaderVerde.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVerde.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.SetMatrix4("model", model);
            _shaderAzul.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderAzul.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderAzul.Use();
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
        }
#endif
    }
}