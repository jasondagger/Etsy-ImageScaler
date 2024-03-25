using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

static partial class Program
{
    public static void Main()
    {
        DisplayCommands();
        Run();
        while (m_isRunning);
    }

    private const float c_versionNumber = 0.1f;

    private static bool m_isRunning = true;

    private static void DisplayCommands()
    {
        Console.WriteLine(
            $"Image Scaler v{c_versionNumber}"
        );
        Console.WriteLine();
        Console.WriteLine(
            "\tFor help: --help"
        );
    }

    private static async void Run()
    {
        while (m_isRunning)
        {
            Console.Write(
                $"\tEnter Command: "
            );

            string? input = Console.ReadLine();
            Console.WriteLine();
            if (input != null)
            {
                List<string> commands = new();
                string inputSplit = string.Empty;
                bool isInQuotes = false;
                foreach (char c in input)
                {
                    if (c == ' ' && !isInQuotes)
                    {
                        commands.Add(
                            inputSplit
                        );
                        inputSplit = string.Empty;
                        continue;
                    }
                    else if (c == '"')
                    {
                        if (isInQuotes)
                        {
                            isInQuotes = false;
                        }
                        else
                        {
                            isInQuotes = true;
                        }
                    }
                    else
                    {
                        inputSplit += c;
                    }
                }
                commands.Add(
                    inputSplit
                );

                string command = commands[0].ToLower();

                switch (command)
                {
                    case "--help":
                        ProcessCommandHelp();
                        break;

                    case "--stop":
                        await ProcessCommandStop();
                        break;

                    default:
                        await Task.Run(
                            () =>
                            {
                                commands.RemoveAt(0);
                                HandleScaleCommands(
                                    command,
                                    commands
                                );
                            }
                        );
                        break;
                }
            }
        }
    }

    private static void HandleScaleCommands(
        string scale,
        List<string> commands
    )
    {
        if (commands.Count > 0)
        {
            while (commands.Count > 0)
            {
                string path = commands[0].ToLower();
                commands.RemoveAt(0);

                switch (scale)
                {
                    case "--u":
                        HandleScaleCommand(
                            path,
                            true
                        );
                        break;

                    case "--d":
                        HandleScaleCommand(
                            path,
                            false
                        );
                        break;

                    default:
                        Console.WriteLine(
                            $"\t\tInvalid Scale Parameters."
                        );
                        Console.WriteLine();
                        break;
                }
            }
        }
        else
        {
            Console.WriteLine(
                $"\t\tInvalid Scale Parameters."
            );
            Console.WriteLine();
        }
    }

    private static void HandleScaleCommand(
        string path,
        bool upScale
    )
    {
        Console.WriteLine(
            $"\t\tLoading Image at {path}..."
        );
        Console.WriteLine();
        try
        {
            using (
                Image<Rgba32> image = Image.Load<Rgba32>(
                    path
                )
            )
            {
                if (image != null)
                {
                    Console.WriteLine(
                        $"\t\t{(upScale ? "Up-Scaling" : "Down-Scaling")} Image..."
                    );

                    Console.WriteLine(
                        $"\t\tInput Width: {image.Width}"
                    );
                    Console.WriteLine(
                        $"\t\tInput Height: {image.Height}"
                    );
                    Console.WriteLine();

                    float scalar = upScale ? 2 : 0.5f;

                    int width = (int)(image.Width * scalar);
                    int height = (int)(image.Height * scalar);

                    image.Mutate(
                        x =>
                        {
                            x.Resize(
                                width,
                                height,
                                KnownResamplers.Lanczos8
                            );
                        }
                    );

                    Console.WriteLine(
                        $"\t\tSaving Image..."
                    );
                    Console.WriteLine();
                    image.Save(
                        path
                    );

                    Console.WriteLine(
                        $"\t\tProcess Completed!"
                    );
                    Console.WriteLine(
                        $"\t\tOutput Width: {width}"
                    );
                    Console.WriteLine(
                        $"\t\tOutput Height: {height}"
                    );
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(
                        $"\t\tInvalid input. Must be an image path. Drag & drop an image to the window."
                    );
                    Console.WriteLine();
                }
            }
        }
        catch
        {
            Console.WriteLine(
                $"\t\tPath given is not valid."
            );
            Console.WriteLine();
        }
    }

    private static void ProcessCommandHelp()
    {
        Console.WriteLine("\tCommand Format:");
        Console.WriteLine("\t\t--parameter1 --p2 ... --pN");
        Console.WriteLine();

        Console.WriteLine("\tApplication:");
        Console.WriteLine("\t\t--help");
        Console.WriteLine("\t\t--stop");
        Console.WriteLine();

        Console.WriteLine(
            "\tImage Scale:"
        );
        Console.WriteLine(
            "\t\t--d path"
        );
        Console.WriteLine(
            "\t\t--u path"
        );
        Console.WriteLine();
    }

    private static async Task ProcessCommandStop()
    {
        Console.WriteLine(
            "\tClosing Application in 3s..."
        );
        await Task.Delay(
            3000
        );
        m_isRunning = false;
    }
};