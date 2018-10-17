/*
 * MIT License
 *
 * Copyright 2018 Marvin Neurath
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Diagnostics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using WpfSharpDxControl;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SampleControl3d {
	/// <summary>
	///     A sample implementation showcasing how to implement 3d DirectX11 rendering in a WPF UserControl.
	///     Influenced by
	///     <see cref="https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/Direct3D11/MiniCube/Program.cs" />
	/// </summary>
	public class Sample3DRenderer : DirectXComponent {
		//unit cube, read as [vertex1_position,vertex1_color,vertex2_position,vertex2_color,....]
		private static readonly Vector4[] vertsPosColor = {
			new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
			new Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, -1.0f, 1.0f),
			new Vector4(1.0f, 0.0f, 0.0f, 1.0f), new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
			new Vector4(1.0f, 1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), new Vector4(1.0f, -1.0f, -1.0f, 1.0f),
			new Vector4(1.0f, 0.0f, 0.0f, 1.0f), new Vector4(-1.0f, -1.0f, 1.0f, 1.0f),
			new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
			new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), new Vector4(-1.0f, 1.0f, 1.0f, 1.0f),
			new Vector4(0.0f, 1.0f, 0.0f, 1.0f), new Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
			new Vector4(1.0f, -1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
			new Vector4(0.0f, 1.0f, 0.0f, 1.0f), new Vector4(-1.0f, 1.0f, -1.0f, 1.0f),
			new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
			new Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
			new Vector4(0.0f, 0.0f, 1.0f, 1.0f), new Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
			new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, -1.0f, 1.0f),
			new Vector4(0.0f, 0.0f, 1.0f, 1.0f), new Vector4(-1.0f, -1.0f, -1.0f, 1.0f),
			new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
			new Vector4(1.0f, -1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), new Vector4(-1.0f, -1.0f, 1.0f, 1.0f),
			new Vector4(1.0f, 1.0f, 0.0f, 1.0f), new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
			new Vector4(1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), new Vector4(1.0f, -1.0f, 1.0f, 1.0f),
			new Vector4(1.0f, 1.0f, 0.0f, 1.0f), new Vector4(-1.0f, -1.0f, -1.0f, 1.0f),
			new Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
			new Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), new Vector4(-1.0f, 1.0f, 1.0f, 1.0f),
			new Vector4(1.0f, 0.0f, 1.0f, 1.0f), new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
			new Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), new Vector4(-1.0f, 1.0f, -1.0f, 1.0f),
			new Vector4(1.0f, 0.0f, 1.0f, 1.0f), new Vector4(1.0f, -1.0f, -1.0f, 1.0f),
			new Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
			new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, -1.0f, 1.0f, 1.0f),
			new Vector4(0.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
			new Vector4(1.0f, 1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
			new Vector4(0.0f, 1.0f, 1.0f, 1.0f)
		};

		//clock to keep track of time passed
		private Stopwatch clock;

		//mvp const buffer
		private Buffer constantBuffer;

		protected override void InternalInitialize() {
			base.InternalInitialize();
			CreateResources();
		}

		private void CreateResources() {
			//compile and set vertexbuffer
			using (var shaderBytecode = ShaderBytecode.CompileFromFile("./Shaders/shader.hlsl", "VS", "vs_4_0")) {
				var signature = ShaderSignature.GetInputSignature(shaderBytecode);
				// Layout from VertexShader input signature
				var layout = new InputLayout(Device, signature,
					new[] {
						new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
						new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
					});


				Device.ImmediateContext.InputAssembler.InputLayout = layout;
				Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

				//create & set vertex buffer
				var vertexBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, vertsPosColor);
				//stride here position+color = SizeOf<Vector4> * 2
				Device.ImmediateContext.InputAssembler.SetVertexBuffers(0,
					new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>() * 2, 0));
				Device.ImmediateContext.VertexShader.Set(new VertexShader(Device, shaderBytecode));
			}

			//compile and set vertexshader
			using (var bytecode = ShaderBytecode.CompileFromFile("./Shaders/shader.hlsl", "PS", "ps_4_0")) {
				Device.ImmediateContext.PixelShader.Set(new PixelShader(Device, bytecode));
			}

			//setup constant buffer for mvp matrix
			constantBuffer = new Buffer(Device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer,
				CpuAccessFlags.None, ResourceOptionFlags.None, 0);
			Device.ImmediateContext.VertexShader.SetConstantBuffer(0, constantBuffer);

			//create a clock to animate in render call
			clock = new Stopwatch();
			clock.Start();
		}

		protected override void Render() {
			//reset render target
			Device.ImmediateContext.ClearRenderTargetView(RenderTargetView, Color.Black);

			//create model-view-projection matrix
			var viewMat = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
			var projMat = Matrix.PerspectiveFovLH((float) Math.PI / 4.0f, SurfaceWidth / (float) SurfaceHeight, 0.1f, 100.0f);

			var time = clock.ElapsedMilliseconds / 1000f;
			var modelMat = Matrix.RotationY(time) * Matrix.RotationX(time * 2) * Matrix.RotationZ(time * 0.7f);
			var mvpMat = modelMat * viewMat * projMat;

			Device.ImmediateContext.UpdateSubresource(ref mvpMat, constantBuffer);
			Device.ImmediateContext.Draw(36, 0);
		}
	}
}