(:action pick
	:parameters (ball1 rooma left)
	:precondition  
		(and  
			(ball ball1) 
			(room rooma) 
			(gripper left)
			(at ball1 rooma) 
			(at-robby rooma) 
			(free left)
		)
	:effect 
		(and 
			(carry ball1 left)
		    (not (at ball1 rooma)) 
		    (not (free left))
		)
	)