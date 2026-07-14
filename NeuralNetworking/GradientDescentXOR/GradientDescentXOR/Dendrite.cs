namespace Program
{
    public class Dendrite
    {
        
        public Neuron previous { get; }
        public Neuron next { get; }
        public double weight { get; set; }
        public double weightUpdate { get; set; }

        public Dendrite(Neuron previous, Neuron next, double weight)
        {
            this.previous = previous;
            this.next = next;
            this.weight = weight;
        }

        public void ApplyUpdates()
        {
            weight += weightUpdate;
            weightUpdate = 0;
        }

        public double Compute()
        {
            return previous.Output * weight;
        }
    }
}