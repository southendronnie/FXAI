  public class PatternLibrary
  {
    public List<string> KnownPatterns { get; set; } = new();

    public bool IsMatch(string pattern)
    {
      return KnownPatterns.Contains(pattern);
    }
  }
