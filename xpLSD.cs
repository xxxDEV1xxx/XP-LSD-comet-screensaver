using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

class LsdFluid : Form
{
    private byte[] palette;
    private Color[] colors;
    private byte[,] colorBytes;
    private Timer timer;
    private Bitmap canvas;
    private int simWidth = 160; // Simulation resolution
    private int simHeight = 100;
    private int bitmapWidth = 320; // Bitmap resolution
    private int bitmapHeight = 200;
    private int displayWidth; // Screen display size
    private int displayHeight;
    private Point lastMousePosition;
    private float[,] density;
    private float[,] velX;
    private float[,] velY;
    private float frameTime = 0;

    public LsdFluid()
    {
        // Initialize palette
        palette = new byte[384] {
            0x3f,0x00,0x3f, 0x3f,0x02,0x3d, 0x3f,0x04,0x3b, 0x3f,0x06,0x39,
            0x3f,0x08,0x37, 0x3f,0x0a,0x35, 0x3f,0x0c,0x33, 0x3f,0x0e,0x31,
            0x3f,0x10,0x2f, 0x3f,0x12,0x2d, 0x3f,0x14,0x2b, 0x3f,0x16,0x29,
            0x3f,0x18,0x27, 0x3f,0x1a,0x25, 0x3f,0x1c,0x23, 0x3f,0x1e,0x21,
            0x3f,0x20,0x1f, 0x3f,0x22,0x1d, 0x3f,0x24,0x1b, 0x3f,0x26,0x19,
            0x3f,0x28,0x17, 0x3f,0x2a,0x15, 0x3f,0x2c,0x13, 0x3f,0x2e,0x11,
            0x3f,0x30,0x0f, 0x3f,0x32,0x0d, 0x3f,0x34,0x0b, 0x3f,0x36,0x09,
            0x3f,0x38,0x07, 0x3f,0x3a,0x05, 0x3f,0x3c,0x03, 0x3f,0x3e,0x01,
            0x3f,0x3f,0x00, 0x3d,0x3f,0x02, 0x3b,0x3f,0x04, 0x39,0x3f,0x06,
            0x37,0x3f,0x08, 0x35,0x3f,0x0a, 0x33,0x3f,0x0c, 0x31,0x3f,0x0e,
            0x2f,0x3f,0x10, 0x2d,0x3f,0x12, 0x2b,0x3f,0x14, 0x29,0x3f,0x16,
            0x27,0x3f,0x18, 0x25,0x3f,0x1a, 0x23,0x3f,0x1c, 0x21,0x3f,0x1e,
            0x1f,0x3f,0x20, 0x1d,0x3f,0x22, 0x1b,0x3f,0x24, 0x19,0x3f,0x26,
            0x17,0x3f,0x28, 0x15,0x3f,0x2a, 0x13,0x3f,0x2c, 0x11,0x3f,0x2e,
            0x0f,0x3f,0x30, 0x0d,0x3f,0x32, 0x0b,0x3f,0x34, 0x09,0x3f,0x36,
            0x07,0x3f,0x38, 0x05,0x3f,0x3a, 0x03,0x3f,0x3c, 0x01,0x3f,0x3e,
            0x00,0x3f,0x3f, 0x00,0x3d,0x3f, 0x00,0x3b,0x3f, 0x00,0x39,0x3f,
            0x00,0x37,0x3f, 0x00,0x35,0x3f, 0x00,0x33,0x3f, 0x00,0x31,0x3f,
            0x00,0x2f,0x3f, 0x00,0x2d,0x3f, 0x00,0x2b,0x3f, 0x00,0x29,0x3f,
            0x00,0x27,0x3f, 0x00,0x25,0x3f, 0x00,0x23,0x3f, 0x00,0x21,0x3f,
            0x00,0x1f,0x3f, 0x00,0x1d,0x3f, 0x00,0x1b,0x3f, 0x00,0x19,0x3f,
            0x00,0x17,0x3f, 0x00,0x15,0x3f, 0x00,0x13,0x3f, 0x00,0x11,0x3f,
            0x00,0x0f,0x3f, 0x00,0x0d,0x3f, 0x00,0x0b,0x3f, 0x00,0x09,0x3f,
            0x00,0x07,0x3f, 0x00,0x05,0x3f, 0x00,0x03,0x3f, 0x00,0x01,0x3f,
            0x00,0x00,0x3f, 0x02,0x00,0x3f, 0x04,0x00,0x3f, 0x06,0x00,0x3f,
            0x08,0x00,0x3f, 0x0a,0x00,0x3f, 0x0c,0x00,0x3f, 0x0e,0x00,0x3f,
            0x10,0x00,0x3f, 0x12,0x00,0x3f, 0x14,0x00,0x3f, 0x16,0x00,0x3f,
            0x18,0x00,0x3f, 0x1a,0x00,0x3f, 0x1c,0x00,0x3f, 0x1e,0x00,0x3f,
            0x20,0x00,0x3f, 0x22,0x00,0x3f, 0x24,0x00,0x3f, 0x26,0x00,0x3f,
            0x28,0x00,0x3f, 0x2a,0x00,0x3f, 0x2c,0x00,0x3f, 0x2e,0x00,0x3f,
            0x30,0x00,0x3f, 0x32,0x00,0x3f, 0x34,0x00,0x3f, 0x36,0x00,0x3f,
            0x38,0x00,0x3f, 0x3a,0x00,0x3f, 0x3c,0x00,0x3f, 0x3f,0x00,0x3f
        };

        colors = new Color[128];
        colorBytes = new byte[128, 3];
        density = new float[simWidth, simHeight];
        velX = new float[simWidth, simHeight];
        velY = new float[simWidth, simHeight];

        // Full-screen, borderless
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.Black;
        this.DoubleBuffered = true;

        Rectangle screen = Screen.PrimaryScreen.Bounds;
        displayWidth = screen.Width;
        displayHeight = screen.Height;

        Cursor.Position = new Point(displayWidth - 1, displayHeight - 1);
        lastMousePosition = Cursor.Position;

        canvas = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);

        // Initialize palette
        for (int i = 0; i < 128; i++)
        {
            int r = palette[i * 3 + 0] * 255 / 63;
            int g = palette[i * 3 + 1] * 255 / 63;
            int b = palette[i * 3 + 2] * 255 / 63;
            colors[i] = Color.FromArgb(r, g, b);
            colorBytes[i, 0] = (byte)b;
            colorBytes[i, 1] = (byte)g;
            colorBytes[i, 2] = (byte)r;
        }

        // Initialize density and velocity
        for (int y = 0; y < simHeight; y++)
        {
            for (int x = 0; x < simWidth; x++)
            {
                float fx = x / (float)simWidth;
                float fy = y / (float)simHeight;
                density[x, y] = (float)(Math.Sin(fx * 10 + fy * 10) * 1.5 + 0.5);
                velX[x, y] = (float)Math.Sin(fx * 15) * 0.1f;
                velY[x, y] = (float)Math.Cos(fy * 15) * 0.1f;
            }
        }

        timer = new Timer();
        timer.Interval = 16; // ~60 FPS
        timer.Tick += new EventHandler(UpdateFrame);
        timer.Start();

        this.MouseMove += new MouseEventHandler(MouseMoveHandler);
        this.MouseClick += new MouseEventHandler(MouseClickHandler);
        this.KeyDown += new KeyEventHandler(KeyDownHandler);

        // Initial canvas fill to avoid black screen
        using (Graphics graphics = Graphics.FromImage(canvas))
        {
            graphics.Clear(Color.FromArgb(palette[0], palette[1], palette[2]));
        }
    }

    private void MouseMoveHandler(object sender, MouseEventArgs e)
    {
        Point current = Cursor.Position;
        if (Math.Abs(current.X - lastMousePosition.X) > 5 || Math.Abs(current.Y - lastMousePosition.Y) > 5)
        {
            Application.Exit();
        }
        lastMousePosition = current;
    }

    private void MouseClickHandler(object sender, MouseEventArgs e)
    {
        Application.Exit();
    }

    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
        Application.Exit();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Scale bitmap to screen with aspect ratio
        float aspectRatio = (float)bitmapWidth / bitmapHeight;
        float screenAspectRatio = (float)displayWidth / displayHeight;
        int drawWidth, drawHeight, xOffset, yOffset;

        if (screenAspectRatio > aspectRatio)
        {
            drawHeight = displayHeight;
            drawWidth = (int)(displayHeight * aspectRatio);
            xOffset = (displayWidth - drawWidth) / 2;
            yOffset = 0;
        }
        else
        {
            drawWidth = displayWidth;
            drawHeight = (int)(displayWidth / aspectRatio);
            xOffset = 0;
            yOffset = (displayHeight - drawHeight) / 2;
        }

        e.Graphics.DrawImage(canvas, xOffset, yOffset, drawWidth, drawHeight);
        base.OnPaint(e);
    }

    private void CreateGradientBackground(Bitmap bmp, float time)
    {
        // Define center and radius for the radial gradient
        float centerX = bitmapWidth / 2f;
        float centerY = bitmapHeight / 2f;
        float maxRadius = (float)Math.Sqrt(centerX * centerX + centerY * centerY);

        // Dynamic colors for the gradient
        float t = time * 0.1f; // Slower animation for smoothness
        int outerR = (int)(Math.Sin(t) * 127 + 128);
        int outerG = (int)(Math.Sin(t + 2) * 127 + 128);
        int outerB = (int)(Math.Sin(t + 4) * 127 + 128);
        int innerR = (int)(Math.Cos(t) * 127 + 128);
        int innerG = (int)(Math.Cos(t + 2) * 127 + 128);
        int innerB = (int)(Math.Cos(t + 4) * 127 + 128);

        // Lock bitmap for direct pixel manipulation
        BitmapData bmpData = bmp.LockBits(
            new Rectangle(0, 0, bitmapWidth, bitmapHeight),
            ImageLockMode.WriteOnly,
            PixelFormat.Format24bppRgb
        );

        unsafe
        {
            byte* ptr = (byte*)bmpData.Scan0;
            int stride = bmpData.Stride;
            for (int y = 0; y < bitmapHeight; y++)
            {
                byte* row = ptr + y * stride;
                for (int x = 0; x < bitmapWidth; x++)
                {
                    float dx = x - centerX;
                    float dy = y - centerY;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    float tValue = Math.Min(distance / maxRadius, 1f); // Normalize distance

                    // Interpolate between outer and inner colors
                    int r = (int)(outerR + (innerR - outerR) * tValue);
                    int g = (int)(outerG + (innerG - outerG) * tValue);
                    int b = (int)(outerB + (innerB - outerB) * tValue);

                    int offset = x * 3;
                    row[offset + 0] = (byte)b; // Blue
                    row[offset + 1] = (byte)g; // Green
                    row[offset + 2] = (byte)r; // Red
                }
            }
        }

        bmp.UnlockBits(bmpData);
    }

private void UpdateFrame(object sender, EventArgs e)
{
    // Add forces to create swirls
    float t = frameTime * 0.35f;
    int cx = (int)(simWidth / 2 + simWidth / 4 * Math.Sin(t * 0.17f));
    int cy = (int)(simHeight / 2 + simHeight / 4 * Math.Cos(t * 0.12f));
    cx = Math.Max(2, Math.Min(simWidth - 5, cx));
    cy = Math.Max(2, Math.Min(simHeight - 5, cy));

    // Apply density and velocity to a 5x5 region to make the object ~4x larger in area
    int radius = 2; // Half of 5x5 region
    for (int dy = -radius; dy <= radius; dy++)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            int nx = cx + dx;
            int ny = cy + dy;
            if (nx >= 0 && nx < simWidth && ny >= 0 && ny < simHeight)
            {
                // Apply density with a falloff to smooth the edges
                float falloff = (float)(1.0 / (1.0 + (dx * dx + dy * dy) * 0.5));
                density[nx, ny] += 1.0f * falloff;
                velX[nx, ny] += (float)Math.Sin(t) * 0.4f * falloff;
                velY[nx, ny] += (float)Math.Cos(t) * 0.4f * falloff;
            }
        }
    }

    // Advect density
    float[,] newDensity = new float[simWidth, simHeight];
    for (int y = 1; y < simHeight - 1; y++)
    {
        for (int x = 1; x < simWidth - 1; x++)
        {
            float px = x - velX[x, y];
            float py = y - velY[x, y];
            int ix = (int)px;
            int iy = (int)py;
            float fx = px - ix;
            float fy = py - iy;
            ix = Math.Max(1, Math.Min(simWidth - 2, ix));
            iy = Math.Max(1, Math.Min(simHeight - 2, iy));
            float d00 = density[ix, iy];
            float d10 = density[ix + 1, iy];
            float d01 = density[ix, iy + 1];
            float d11 = density[ix + 1, iy + 1];
            float d = (1 - fx) * (1 - fy) * d00 + fx * (1 - fy) * d10 +
                      (1 - fx) * fy * d01 + fx * fy * d11;
            newDensity[x, y] = Math.Max(0, Math.Min(1, d));
        }
    }
    density = newDensity;

    // Diffuse density
    for (int y = 1; y < simHeight - 1; y++)
    {
        for (int x = 1; x < simWidth - 1; x++)
        {
            density[x, y] = (density[x, y] +
                             (density[x - 1, y] + density[x + 1, y] +
                              density[x, y - 1] + density[x, y + 1]) * 0.2f) * 0.5f;
        }
    }

    // Apply gradient background
    CreateGradientBackground(canvas, frameTime);

    // Update bitmap with fluid simulation (blend with gradient)
    BitmapData bmpData = canvas.LockBits(
        new Rectangle(0, 0, bitmapWidth, bitmapHeight),
        ImageLockMode.ReadWrite,
        PixelFormat.Format24bppRgb
    );

    unsafe
    {
        byte* ptr = (byte*)bmpData.Scan0;
        int stride = bmpData.Stride;
        for (int y = 0; y < bitmapHeight; y++)
        {
            byte* row = ptr + y * stride;
            int sy = y * simHeight / bitmapHeight;
            for (int x = 0; x < bitmapWidth; x++)
            {
                int sx = x * simWidth / bitmapWidth;
                float d = density[sx, sy];
                byte colorIndex = (byte)((d + 0.2f * (float)Math.Sin(t * 0.05f)) * 127);
                int offset = x * 3;

                // Blend fluid color with background
                byte fluidR = colorBytes[colorIndex % 128, 2];
                byte fluidG = colorBytes[colorIndex % 128, 1];
                byte fluidB = colorBytes[colorIndex % 128, 0];
                byte bgR = row[offset + 2];
                byte bgG = row[offset + 1];
                byte bgB = row[offset + 0];
                float alpha = d; // Use density as blending factor
                row[offset + 2] = (byte)(fluidR * alpha + bgR * (1 - alpha));
                row[offset + 1] = (byte)(fluidG * alpha + bgG * (1 - alpha));
                row[offset + 0] = (byte)(fluidB * alpha + bgB * (1 - alpha));
            }
        }
    }

    canvas.UnlockBits(bmpData);

    frameTime += 1.0f;

    this.Invalidate();
}

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new LsdFluid());
    }
}