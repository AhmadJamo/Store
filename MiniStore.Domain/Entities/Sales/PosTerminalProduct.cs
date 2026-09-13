namespace MiniStore.Domain.Entities;
public class PosTerminalProduct
{
    public int PosTerminalId { get; private set; }
    public int ProductId { get; private set; }
    private PosTerminalProduct() { }
    public PosTerminalProduct(int posTerminalId, int productId) { if (posTerminalId <= 0 || productId <= 0) throw new ArgumentException("POS terminal and product are required."); PosTerminalId = posTerminalId; ProductId = productId; }
}
