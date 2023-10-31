(define (domain gripper-strips)
   (:predicates
		(room ?r)
		(gripper ?g)
		(ball ?b)
		(at-robby ?r)
		(at ?b ?r)
		(free ?g)
		(carry ?o ?g))

   (:action move
       :parameters  (?from ?to)
       :precondition (and (at-robby ?from) (room ?from) (room ?to) )
       :effect (and  (at-robby ?to)
		     (not (at-robby ?from))))

   (:action pick
       :parameters (?obj ?room ?gripper)
       :precondition  (and 
			    (at ?obj ?room) (at-robby ?room) (free ?gripper)
				(ball ?obj)
				(room ?room)
				(gripper ?gripper)
				)
       :effect (and (carry ?obj ?gripper)
		    (not (at ?obj ?room)) 
		    (not (free ?gripper))))


   (:action drop
       :parameters  (?obj ?room ?gripper)
       :precondition  (and 
			    (carry ?obj ?gripper) (at-robby ?room)
				(ball ?obj)
				(room ?room)
				(gripper ?gripper))
       :effect (and (at ?obj ?room)
		    (free ?gripper)
		    (not (carry ?obj ?gripper)))))

