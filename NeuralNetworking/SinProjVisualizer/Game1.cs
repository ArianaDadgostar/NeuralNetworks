using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MonoGame.Extended;
using System.Linq;

namespace SinProjVisualizer;

public class Dendrite
{
    public Neuron previous { get; }
    public Neuron next { get; }
    public double weight { get; set; }

    public Dendrite(Neuron previous, Neuron next, double weight)
    {
        this.previous = previous;
        this.next = next;
        this.weight = weight;
    }

    public double Compute()
    {
        return previous.Output * weight;
    }
}

public class Neuron
{
    double bias;
    public Dendrite[] dendrites;
    public double Input { get; }
    public double Output { get; set; }
    public ActivationFunc Activation ;

    public Neuron(ActivationFunc activation, Neuron[] previousNeurons)
    {
        Activation = activation;
        dendrites = new Dendrite[previousNeurons.Length];
        for(int i = 0; i < dendrites.Length; i ++)
        {
            dendrites[i] = new Dendrite(previousNeurons[i], this, 0);
        }
    }

    public double Compute()
    {
        double sum = bias;
        foreach (var d in dendrites)
            sum += d.Compute();

        Output = Activation.Compute(sum);
        if(Output != 0)
        {
            ;
        }
        return Output;
    }


    public void Randomize(Random random)
    {
        for(int i = 0; i < dendrites.Length; i++)
        {
            dendrites[i].weight = random.NextDouble() /10;
        }
        bias = random.Next(0, 1) == 0 ? random.NextDouble() /10 : random.NextDouble()/10;
    }
}

public class Layer
{
    public Neuron[] neurons;
    public double[] outputs;

    public Layer(ActivationFunc activation, int neuronCount, Layer previousLayer)
    {
        neurons = new Neuron[neuronCount];
        outputs = new double[neuronCount];
        for(int i = 0; i < neuronCount; i++)
        {
            neurons[i] = (previousLayer == null) ? new Neuron(activation, new Neuron[0]) : new Neuron(activation, previousLayer.neurons);
            neurons[i].Activation = activation;
            for(int j = 0; j < neurons[i].dendrites.Length; j++)
            {
                neurons[i].dendrites[j] = new Dendrite(previousLayer.neurons[j], neurons[i], 0);
            }
        }
    }

    public void Calculate()
    {
        for(int i = 0; i < neurons.Length; i ++)
        {
            neurons[i].Compute();
            outputs[i] = neurons[i].Output;
        }
    }

    public void Randomize(Random random)
    {
        for(int i = 0; i < neurons.Length; i++)
        {
            neurons[i].Randomize(random);
        }
    }
}

public class Network
{
    public Layer[] layers;
    public double[] outputs;
    public Action<int> ErrorCalculation;

    public void Randomize()
    {
        Random random = new Random();
        for(int i = 0; i < layers.Length; i ++)
        {
            layers[i].Randomize(random);
        }
    }

    public void Compute(double[] inputs) 
    {
        layers[0].outputs = inputs;
        layers[0].neurons[0].Output = inputs[0];
        for(int i = 0; i < layers.Length; i++)
        {
            layers[i].Calculate();
        }
        outputs = layers[^1].outputs;
    }
}

public class ActivationFunc
{
    public Func<double, double> activation;
    public Func<double, double> derivative;

    public ActivationFunc(Func<double, double> activation, Func<double, double> derivative)
    {
        this.activation = activation;
        this.derivative = derivative;
    }

    public static double Function(double input)
    {
        if(input > 0) return 1;
        return 0;
    }

    public static double SinFunc(double input)
    {
        return input;
    }

    public double Compute(double input)
    {
        return activation(input);
    }

    public static double Derivative()
    {
        return 1;
    }


    public static double Derivative(double input)
    {
        return 1;
    }
}

public class SinProj
{
    public Network[] networks;

    public SinProj(int length)
    {
        networks = new Network[length];
        for(int i = 0; i < networks.Length; i ++)
        {
            networks[i] = new Network();
            networks[i].layers = new Layer[3];
            networks[i].layers[0] = new Layer(new ActivationFunc(ActivationFunc.SinFunc, ActivationFunc.Derivative), 1, null);
            for(int j = 1; j < networks[i].layers.Length; j++)
            {
                ActivationFunc acti = new ActivationFunc(ActivationFunc.SinFunc, ActivationFunc.Derivative);
                if(j == networks[i].layers.Length - 1)
                {
                    networks[i].layers[j] = new Layer(acti, 1, networks[i].layers[j - 1]);
                }
                else
                networks[i].layers[j] = new Layer(acti, networks.Length, networks[i].layers[j - 1]);
                //(-1 * Math.Abs(j - 1)) + 2
            }
        }
    }

    public double FitnessFunc(double[] inputs, Network network)
    {
        network.layers[0].neurons[0].Output = inputs[0];
        network.Compute(inputs);

        double average = 0;

        for(int i = 0; i < network.outputs.Length; i ++)
        {
            double expected = Math.Sin(inputs[i]);
            average += expected - network.outputs[i];
        }
        return average/network.outputs.Length;
    }

    public PriorityQueue<Network, double> Sort(double[] inputs)
    {
        PriorityQueue<Network, double> population = new PriorityQueue<Network, double>();
        for(int j = 0; j < networks.Length; j++)
        {
            for(int i = 0; i < inputs.Length; i++)
            {
                foreach(Layer layer in networks[j].layers)
                {
                    networks[j].Compute(new double[] { inputs[i] });
                }
            }
            population.Enqueue(networks[j], Math.Abs(FitnessFunc(inputs, networks[j])));
        }

        return population;
    }

    public void Mutate(double[] inputs)
    {
        PriorityQueue<Network, double> population = Sort(inputs);
        Network[] top = new Network[(int)(population.Count * 0.1)];
        Network[] bottom = new Network[top.Length];
        Network[] middle = new Network[population.Count - (top.Length + bottom.Length)];

        int popCount = population.Count;

        for(int i = 0; i < popCount; i ++)
        {
            if(i < top.Length)
            {
                top[i] = population.Dequeue();
                continue;
            }
            else if(i < middle.Length + top.Length)
            {
                middle[i - top.Length] = population.Dequeue();
                continue;
            }

            bottom[i-(top.Length + middle.Length)] = population.Dequeue();
        }

        CrossOver(top, middle);
        for(int i = 0; i < bottom.Length; i++)
        {
            bottom[i].Randomize();
        }
    }

    public void CrossOver(Network[] top, Network[] middle)
    {
        Random random = new Random();
        for(int w = 0; w < top.Length; w++)
        {
            for(int l = 0; l < top[w].layers.Length; l ++)
            {
                for(int n = 0; n < top[w].layers[l].neurons.Length; n ++)
                {
                    double split = random.Next(top[w].layers[l].neurons[n].dendrites.Length);
    
                    for(int d = 0; d < top[w].layers[l].neurons[n].dendrites.Length; d++)
                    {
                        if(d < split)
                        {
                            middle[w].layers[l].neurons[n].dendrites[d].weight = top[w].layers[l].neurons[n].dendrites[d].weight;
                        }
                    }
                }
            }
        }
    }
    
    public double Train(double[] inputs)
    {
        Mutate(inputs);
        double averageFitness = 0;
        foreach(Network network in networks)
        {
            averageFitness += FitnessFunc(inputs, network);
        }
        return averageFitness / networks.Length;
    }
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;


    bool counter = true;

    public double[] results;
    public double[] inputs;
    Network network;

    SpriteFont font;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }
    SinProj sinProj;
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        sinProj = new SinProj(10);
        inputs = new double[sinProj.networks.Length];
        results = new double[inputs.Length];
        Random random = new Random();
        for(int i = 0; i < inputs.Length; i ++)
        {  
            inputs[i] = (double)random.NextDouble();
        }

        //font = Content.Load<SpriteFont>("Arial");

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        double result = sinProj.Train(inputs);
        PriorityQueue<Network, double> population = sinProj.Sort(inputs);
        results = population.Dequeue().outputs;

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        counter = !counter;

        for(int i = 0; i < results.Length; i++)
        {
            float x = (float)(inputs[i] * 400 + 400); // Scale and shift to fit the window
            float y = (float)(results[i] * 400 + 300); // Scale and invert for drawing
            float expectedY = (float)(Math.Sin(inputs[i] * 4) * 100 + 300); // Expected output for comparison  
            _spriteBatch.Begin();
            _spriteBatch.DrawRectangle(new Rectangle((int)x, (int)y, 5, 5), Color.White);
            _spriteBatch.DrawRectangle(new Rectangle((int)x, (int)expectedY, 5, 5), Color.Red);
            Color color = counter ? Color.Green : Color.Blue;
            _spriteBatch.DrawRectangle(new Rectangle((int)0, (int)0, 30, 30), color);
            _spriteBatch.End();
        }

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
