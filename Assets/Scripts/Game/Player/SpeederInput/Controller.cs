using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Action = System.Action;


namespace Player.SpeederInput
{
    public class Controller
    {
        public static Controller Instance { get; private set; }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            Any,
            None
        }

        public enum Modifier
        {
            Shift,
            Caps,
            Space,
            None
        }

        // Directions
        public bool IsPushingLeft => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        public bool IsPushingRight => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        public bool IsPushingUp => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        public bool IsPushingDown => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        // Modifiers
        public bool IsPushingShift => Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
        public bool IsPushingCaps => Input.GetKey(KeyCode.CapsLock);
        public bool IsPushingSpace => Input.GetKey(KeyCode.CapsLock);


        public bool IsPushingNone => !IsPushingLeft && !IsPushingRight && !IsPushingUp && !IsPushingDown;
        public bool IsAnyPushing => IsPushingLeft || IsPushingRight || IsPushingUp || IsPushingDown;

        public float HorizontalInput => Input.GetAxis("Horizontal");

        static Controller()
        {
            Instance = new Controller();
        }

        private Controller()
        {
        }

        private (bool fired, Direction direction, Modifier modifier) HandleMovementSwitch(Direction direction,
            Modifier modifier)
        {
            return (direction, modifier) switch
            {
                (Direction.Down, Modifier.None) when IsPushingDown =>
                    (true, Direction.Down, Modifier.None),

                (Direction.Right, Modifier.None) when IsPushingRight =>
                    (true, Direction.Right, Modifier.None),

                (Direction.Left, Modifier.None) when IsPushingLeft =>
                    (true, Direction.Left, Modifier.None),

                (Direction.Up, Modifier.None) when IsPushingUp =>
                    (true, Direction.Up, Modifier.None),

                (Direction.None, Modifier.None) when IsPushingNone =>
                    (true, Direction.None, Modifier.None),

                (Direction.Any, Modifier.None) when IsAnyPushing =>
                    (true, Direction.Any, Modifier.None),

                //
                (Direction.Down, Modifier.Shift) when IsPushingDown && IsPushingShift =>
                    (true, Direction.Down, Modifier.Shift),

                (Direction.Right, Modifier.Shift) when IsPushingRight && IsPushingShift =>
                    (true, Direction.Right, Modifier.Shift),

                (Direction.Left, Modifier.Shift) when IsPushingLeft && IsPushingShift =>
                    (true, Direction.Left, Modifier.Shift),

                (Direction.Up, Modifier.Shift) when IsPushingUp && IsPushingShift =>
                    (true, Direction.Up, Modifier.Shift),
                
                (Direction.None, Modifier.Shift) when IsPushingNone && IsPushingShift =>
                    (true, Direction.Up, Modifier.Shift),
                
                // 
                (Direction.Down, Modifier.Caps) when IsPushingDown && IsPushingCaps =>
                    (true, Direction.Down, Modifier.Shift),

                (Direction.Right, Modifier.Caps) when IsPushingRight && IsPushingCaps =>
                    (true, Direction.Right, Modifier.Shift),

                (Direction.Left, Modifier.Caps) when IsPushingLeft && IsPushingCaps =>
                    (true, Direction.Left, Modifier.Shift),

                (Direction.Up, Modifier.Caps) when IsPushingUp && IsPushingCaps =>
                    (true, Direction.Up, Modifier.Shift),

                (Direction.None, Modifier.Caps) when IsPushingNone && IsPushingCaps =>
                    (true, Direction.Up, Modifier.Shift),

                _ => (false, Direction.None, Modifier.None)
            };
        }

        public SpeederActionResult HandleMovement(Movement movement)
        {
            var (fired, direction, modifier) = HandleMovementSwitch(movement.Direction, movement.Modifier);

            if (fired)
            {
                var actionResult = new SpeederActionResult
                {
                    Fired = true,
                    Direction = direction,
                    Modifier = modifier
                };
                movement.Action();
                return actionResult;
            }

            return new SpeederActionResult { Fired = false };
        }

        public SpeederActionResult<T> HandleMovement<T>(Movement<T> movement)
        {
            var callable = movement.Action;
            var (fired, direction, modifier) = HandleMovementSwitch(movement.Direction, movement.Modifier);

            if (fired)
            {
                var actionResult = new SpeederActionResult<T>
                {
                    Fired = true,
                    Result = callable(),
                    Direction = direction,
                    Modifier = modifier
                };
                return actionResult;
            }

            return new SpeederActionResult<T> { Fired = false };
        }


        // Runs all the actions until the first 
        public SpeederActionResult<T> RunMovements<T>(IEnumerable<Movement<T>> movements) =>
            movements.Select(HandleMovement).FirstOrDefault(a => a.Fired);

        [CanBeNull]
        public SpeederActionResult RunMovements(IEnumerable<Movement> movements) =>
            movements.Select(HandleMovement).FirstOrDefault(a => a.Fired);
    }
}