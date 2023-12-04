using System.Linq;
using UnityEngine;

public class TransformGrabBag
{
    private Transform[] _transforms;
    private Transform[] _shuffledTransforms;
    private int _currentIndex = 0;

    public TransformGrabBag(Transform[] transforms)
    {
        _transforms = transforms;
        ShuffleTransforms();
    }

    public Transform Grab()
    {
        // If we've gone through all Transforms, shuffle them again.
        if (_currentIndex >= _shuffledTransforms.Length)
        {
            ShuffleTransforms();
            _currentIndex = 0;
        }

        // Grab the next Transform and move on to the next one for the future.
        return _shuffledTransforms[_currentIndex++];
    }

    private void ShuffleTransforms()
    {
        // Store the last transform if there was one.
        Transform lastTransform = (_currentIndex > 0 && _currentIndex == _shuffledTransforms.Length) ? _shuffledTransforms[_currentIndex - 1] : null;

        // Shuffle all transforms.
        _shuffledTransforms = _transforms.OrderBy(t => UnityEngine.Random.value).ToArray();

        // If the last transform was selected first in the shuffle, swap it with the second one.
        if (_shuffledTransforms.Length > 1 && _shuffledTransforms[0] == lastTransform)
        {
            _shuffledTransforms[0] = _shuffledTransforms[1];
            _shuffledTransforms[1] = lastTransform;
        }
    }

}
