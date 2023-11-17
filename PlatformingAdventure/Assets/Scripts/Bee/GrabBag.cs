using System.Linq;

public class GrabBag<T> where T : class
{
    T[] _things;
    T[] _shuffledThings;
    int _currentIndex = 0;

    public GrabBag(T[] things)
    {
        _things = things;
        ShuffleThings();
    }

    public T Grab()
    {
        // If we've gone through all Transforms, shuffle them again.
        if (_currentIndex >= _shuffledThings.Length)
        {
            ShuffleThings();
            _currentIndex = 0;
        }

        // Grab the next Transform and move on to the next one for the future.
        return _shuffledThings[_currentIndex++];
    }

    void ShuffleThings()
    {
        // Store the last transform if there was one.
        T lastThing = (_currentIndex > 0 && _currentIndex == _shuffledThings.Length) ? _shuffledThings[_currentIndex - 1] : null;

        // Shuffle all transforms.
        _shuffledThings = _things.OrderBy(t => UnityEngine.Random.value).ToArray();

        // If the last transform was selected first in the shuffle, swap it with the second one.
        if (_shuffledThings.Length > 1 && _shuffledThings[0] == lastThing)
        {
            _shuffledThings[0] = _shuffledThings[1];
            _shuffledThings[1] = lastThing;
        }
    }

}