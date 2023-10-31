(:action drop
	:parameters  (ball1  roomb left)
    :precondition  
		(and  
			(ball ball1) 
			(room roomb) 
			(gripper left)
			(carry ball1 left) 
			(at-robby roomb)
		)
    :effect 
		(and 
			(at ball1 roomb)
			(free left)
			(not (carry ball1 left))
		)
	)