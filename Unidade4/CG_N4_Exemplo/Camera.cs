using System;
using OpenTK.Mathematics; // Necessário para Vector3 e Matrix4

public class Camera
{
    public Vector3 Position { get; set; }
    public Vector3 Front { get; set; } = -Vector3.UnitZ;
    public Vector3 Up { get; set; } = Vector3.UnitY;
    public Vector3 Right { get; set; }

    public float Pitch { get; set; }
    public float Yaw { get; set; } = -MathHelper.PiOver2; // Inicia apontando para frente

    public float AspectRatio { private get; set; }

    public Camera(Vector3 position, float aspectRatio)
    {
        Position = position;
        AspectRatio = aspectRatio;
        UpdateCameraVectors();
    }

    public void UpdateCameraVectors()
    {
        float pitchRadians = MathHelper.DegreesToRadians(Pitch);
        float yawRadians = MathHelper.DegreesToRadians(Yaw);

        Vector3 front;
        front.X = MathF.Cos(pitchRadians) * MathF.Cos(yawRadians);
        front.Y = MathF.Sin(pitchRadians);
        front.Z = MathF.Cos(pitchRadians) * MathF.Sin(yawRadians);
        Front = Vector3.Normalize(front);

        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + Front, Up);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, AspectRatio, 0.1f, 100f);
    }
}
