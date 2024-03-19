// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// namespace Player.Motion
// {
//     public class SpeederTransform : MonoBehaviour
//     {
//         public Transform T;
//         private Dictionary<int, List<Vector3>> _positionMap = new();
//         private Dictionary<int, List<Quaternion>> _rotationMap = new();
//         private Dictionary<int, List<Vector3>> _scaleMap = new();
//         public int MaxStored { get; set; } = Int32.MaxValue;
//         public bool IsReplaying { get; private set; }
//
//         public SpeederTransform()
//         {
//             T = GameObject.FindGameObjectWithTag("Landspeeder").transform;
//         }
//
//         private void SetPosition(Vector3 pos)
//         {
//             var frame = Time.frameCount;
//             T.position = pos;
//             AppendToDictionary(_positionMap, frame, pos);
//             AppendToDictionary(_rotationMap, frame, T.rotation);
//             AppendToDictionary(_scaleMap, frame, T.localScale);
//             TrimDictionary(this._positionMap);
//         }
//
//         private void SetScale(Vector3 scale)
//         {
//             var frame = Time.frameCount;
//             T.localScale = scale;
//             AppendToDictionary(_positionMap, frame, T.position);
//             AppendToDictionary(_rotationMap, frame, T.rotation);
//             AppendToDictionary(_scaleMap, frame, scale);
//             TrimDictionary(this._scaleMap);
//         }
//
//         private void SetRotation(Quaternion rotation)
//         {
//             var frame = Time.frameCount;
//             T.rotation = rotation;
//             AppendToDictionary(_positionMap, frame, T.position);
//             AppendToDictionary(_rotationMap, frame, rotation);
//             AppendToDictionary(_scaleMap, frame, T.localScale);
//             TrimDictionary(this._rotationMap);
//         }
//
//         public Vector3 position
//         {
//             get => T.position;
//             set => SetPosition(value);
//         }
//
//         public Quaternion rotation
//         {
//             get => T.rotation;
//             set => SetRotation(value);
//         }
//
//         public Vector3 scale
//         {
//             get => T.localScale;
//             set => SetScale(value);
//         }
//
//         public SpeederMotion GetMotionsToFrom(int fromFrame, int toFrame)
//         {
//             Func<KeyValuePair<int, List<TValue>>, bool> condition<TValue>(int start, int end) =>
//                 pair => pair.Key >= start && pair.Key <= end;
//
//             return new()
//             {
//                 positions = _positionMap.Where(condition<Vector3>(fromFrame, toFrame)).Select(pair => pair.Value)
//                     .ToList(),
//                 rotations = _rotationMap.Where(condition<Quaternion>(fromFrame, toFrame)).Select(pair => pair.Value)
//                     .ToList(),
//                 scales = _scaleMap.Where(condition<Vector3>(fromFrame, toFrame)).Select(pair => pair.Value).ToList(),
//                 NFrames = fromFrame - toFrame
//             };
//         }
//
//         public SpeederMotion GetMotionsFromToNow(int fromFrame) => this.GetMotionsToFrom(fromFrame, Time.frameCount);
//
//         public void TrimDictionary<TValue>(Dictionary<int, List<TValue>> dictionary)
//         {
//             if (dictionary.Count > MaxStored)
//             {
//                 int smallestKey = dictionary.Keys.Min();
//                 int largestKey = dictionary.Keys.Max();
//                 while (largestKey - smallestKey >= MaxStored)
//                 {
//                     dictionary.Remove(smallestKey);
//                     smallestKey = dictionary.Keys.Min();
//                 }
//             }
//         }
//
//         public void Replay(SpeederMotion motion, bool reverse = false)
//         {
//             if (IsReplaying)
//                 return;
//             StartCoroutine(ReplayCoroutine(motion, reverse));
//         }
//
//         private IEnumerator ReplayCoroutine(SpeederMotion motion, bool reverse)
//         {
//             IsReplaying = true;
//
//             int frameCount = motion.positions.Count;
//
//             for (int i = 0; i < frameCount; i++)
//             {
//                 int index = reverse ? frameCount - 1 - i : i;
//
//                 List<Vector3> positions = motion.positions[index];
//                 List<Quaternion> rotations = motion.rotations[index];
//                 List<Vector3> scales = motion.scales[index];
//
//                 int updateCount = positions.Count;
//
//                 for (int j = 0; j < updateCount; j++)
//                 {
//                     T.position = positions[j];
//                     T.rotation = rotations[j];
//                     T.localScale = scales[j];
//
//                     yield return null;
//                 }
//             }
//
//             IsReplaying = false;
//         }
//
//         private void AppendToDictionary<TValue>(Dictionary<int, List<TValue>> dictionary, int key, TValue value)
//         {
//             if (dictionary.ContainsKey(key))
//             {
//                 dictionary[key].Add(value);
//             }
//             else
//             {
//                 dictionary[key] = new List<TValue> { value };
//             }
//         }
//     }
// }