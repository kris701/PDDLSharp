(:action drop
	:parameters  (ball2  roomb left)
    :precondition  
		(and  
			(ball ball2) 
			(room roomb) 
			(gripper left)
			(carry ball2 left) 
			(at-robby roomb)
		)
    :effect 
		(and 
			(at ball2 roomb)
			(free left)
			(not (carry ball2 left))
		)
	)