(define (problem strips-gripper-x-1)
   (:domain gripper-strips)
   (:objects rooma roomb)
   (:init (room rooma)
          (room roomb)
          )
   (:goal (and (at ball2 roomb)
               (at ball1 roomb))))