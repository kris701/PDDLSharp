(:action pick
	:parameters (ball2 roomb left)
	:precondition  
		(and  
			(ball ball2) 
			(room roomb) 
			(gripper left)
			(at ball2 roomb) 
			(at-robby roomb) 
			(free left)
		)
	:effect 
		(and 
			(carry ball2 left)
		    (not (at ball2 roomb)) 
		    (not (free left))
		)
	)