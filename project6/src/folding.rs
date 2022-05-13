/*
    The input will be a HP-String, aka a string consisting of either the character 'h' or 'p'.

    Step 1:
    -------------------------
    Iterating over the string, note all the 'h' characters at an even or odd index, starting from 0.
    Output should be something along the lines of a new array (or other datastructure) indicating whether each index is Even, Odd, or nothing,
    indicating that the character was a 'p'.
    
    Optimization ideas:
    Since the characters in the HP string is binary (either 'h' or 'p'), it can be converted to maybe a bool, enum or integer, for faster performance.

    Step 2:
    -------------------------
    Having two pointers, starting from both sides of the new 'Even Odd' array, match even with odds respectively from both pointers, or odds with evens
    respectively with both pointers. For each match in each case, have a running counter of matches for the cases, and at termination, pick the case with
    most matches.

    Optimization ideas:
    Instead of iterating through the array twice, keep a counter of both cases and count them both in the same pass, to avoid doing two passes over the array.
    I think solving it recursively with pattern matching is concise and performant.

    Step 3:
    -------------------------
    Find a split in the HP-string, meaning an index that splits the string into two substrings S1 and S2, such that S = S1S2. The split should fulfill either
    of these two criterias:
        1. The amount of 'Evens' in the substring S1 should be bigger or equal than half of the total 'Evens' in the whole string S (in other words, S1 should contain
           more 'Evens' than S2).
           Likewise, the amount of 'Odds' in the substring S2 should be bigger or equal than half of the total 'Odds' in the whole string S (same idea).

        2. The same ideas as in criteria 1., but the string are switched (i.e. S2 contains more or equal 'Evens' than half of the total, and S1 contains more
           or equal 'Odds' than half of the total in S).
    Output should be the two substrings S1 and S2, fulfilling either of the criterias.

    Optimization ideas:
    To make this more performant, it would be a good idea to compute an 'accumulator' array from step 2, that gives the number of 'Evens' and 'Odds' seen so far
    for a certain index (i.e. all indexes up to 10 have 3 'Evens' and 4 'Odds'). This will checking the criteria for partitioning the string take constant time.
    
    A good approach would be to start from somewhere in the 'middle', meaning somewhere in the array that intersects all the matches. From here, we know all the
    equal out (i.e., among all the matches, there are an equal amount of 'Evens' and 'Odds' on either side of the start). Count the number of unmatched 'Evens' and
    'Odds' to determine if the pointer should be moved. If the split fulfills either of the criterias, then return. Else, you know you can either move left or right
    to remove an 'Even' or 'Odd' from one of the substrings (depending on which match you got from step 2). Make sure to also note the unmatched 'Evens' and 'Odds' when
    doing this, to find a partition that fulfills either criteria.

    Step 4:
    -------------------------
    After finding the partitioning of the string S into substrings S1 and S2, the next step is to make a fold of the two substrings. First, align the two substrings
    such that the last index of S1 matches with the first index of S2, and so on (note this may not align perfectly, as the size of S1 and S2 are not necesarrily equal).
    From here, the substrings can be 'folded', essentially skipping 2x characters in either substring, for x >= 1. The goal is to fold the substrings optimally, such that
    the maximum number of 'h' characters are aligned next to each convert_string_to_struct(str: String) -> HP {
    
}
            1. Two unmatched 'h' characters are in the same substring. If this is the case, the length between them determines whether or not they can be matched. More
               consicely, there should be an even number of characters between them, in order for them to match.

            2. Two unmatched 'h' characters are in either substring. Here you need to think kekw.
*/

#[derive(Clone)]
enum Symbol {
    H,
    P
}

#[derive(Clone)]
enum Parity {
    Even,
    Odd
}

struct HP {
    symbols: Vec<Symbol>,
    parities: Vec<Option<Parity>>,
    even_matches_odd: usize,
    odd_matches_even: usize
}

impl HP {
    fn new(str: String) -> HP {
        let lower = str.to_lowercase();
        let symbols = lower
            .chars()
            .map(
                |c|
                match c {
                    'h' => Symbol::H,
                    'p' => Symbol::P,
                    _ => panic!("Unexpected character in HP string: {:?}", c)
                }
            )
            .collect::<Vec<Symbol>>();
        HP { symbols: symbols, ..Default::default() }
    }

    fn get_parities(self) -> HP {
        let parities = self.symbols
            .iter()
            .enumerate()
            .map(
                |(i, s)|
                match s {
                    Symbol::H => {
                        if i % 2 == 0 {
                            Some(Parity::Even)
                        }
                        else {
                            Some(Parity::Odd)
                        }
                    },
                    Symbol::P => None
                }
            ).collect::<Vec<Option<Parity>>>();
        HP {
            symbols: self.symbols,
            parities: parities,
            ..Default::default()
        }
    }

    fn split_parities_in_half(self) -> (Vec<Option<Parity>>, Vec<Option<Parity>>) {
        let len = self.parities.len();
        let middle = len / 2;
        let (left, right) = if len % 2 == 0 {
            // Even
            let left = self.parities[0..middle].to_vec();
            let right = self.parities[middle..len].to_vec();
            println!("{:?}", right.len());
            (left, right)
        } else {
            // Odd
            let left = self.parities[0..middle].to_vec();
            let right = self.parities[middle+1..len].to_vec();
            (left, right)
        };

        // TODO: There must be some better way of doing this
        let mut mut_right = right;
        mut_right.reverse();
        let imut_right = (&*mut_right).to_vec();

        (left, imut_right)
    }

    fn get_maximum_matches(self) {
        let mut even_matches_odd: usize = 0;
        let mut odd_matches_even: usize = 0;
        
        let (left, right) = self.split_parities_in_half();
        let zip = Iterator::zip(left.iter(), right.iter());
        zip.for_each(
            |(l, r)|
            match (l, r) {
                (Some(Parity::Even), Some(Parity::Odd)) => even_matches_odd += 1,
                (Some(Parity::Odd), Some(Parity::Even)) => odd_matches_even += 1,
                _ => ()
            }
        );
        
        // TODO: Find a suitable data structure for holding matches in
        if even_matches_odd > odd_matches_even {
            
        }
        else {

        }
    }
}

impl Default for HP {
    fn default() -> HP {
        HP { 
            symbols: Vec::new(),
            parities: Vec::new(),
            even_matches_odd: 0,
            odd_matches_even: 0
        }
    }
}

#[cfg(test)]
mod tests {
    use crate::folding::HP;
    use crate::folding::Parity;

    #[test]
    fn create_hp_struct_with_legal_lowercase_characters() {
        let str = String::from("hpphppphphpphphpphphppphpphpphphpphp");
        let _ = HP::new(str);
        assert!(1 == 1); // dummy assert, method was successful if hp struct was created
    }

    #[test]
    fn create_hp_struct_with_legal_uppercase_characters() {
        let str = String::from("HPPPHPHPPPHPPPHPPPHPPPHPPHPHPPHPH");
        let _ = HP::new(str);
        assert!(1 == 1); // dummy assert, method was successful if hp struct was created
    }

    #[test]
    fn create_hp_struct_with_legal_uppercase_and_lowercase_characters() {
        let str = String::from("HPPPHPHPPPHPhppphhphpphpHPPPHPHPHPHPPhphphhphp");
        let _ = HP::new(str);
        assert!(1 == 1); // dummy assert, method was successful if hp struct was created
    }

    #[test]
    #[should_panic]
    fn fail_to_create_hp_struct_with_illegal_characters() {
        let str = String::from("this is not a legal string");
        let _ = HP::new(str);
    }

    #[test]
    fn parity_of_all_p_is_all_none() {
        let str = String::from("pppppppppppppppppp");
        let hp = HP::new(str)
            .get_parities();

        hp.parities
            .iter()
            .for_each(|x| -> () {
                assert!(x.is_none())
            });
    }

    #[test]
    fn parity_of_all_h_is_all_some() {
        let str = String::from("hhhhhhhhhhhhhhhhhh");
        let hp = HP::new(str)
            .get_parities();

        hp.parities
            .iter()
            .for_each(|x| -> () {
                assert!(x.is_some())
            });
    }

    #[test]
    fn parity_of_combo_is_both_some_and_none() {
        let str = String::from("hphphphpphpppphhhpp");
        let hp = HP::new(str)
            .get_parities();
        
        let exists_none = hp.parities.iter().any(|x| x.is_none());
        let exists_some = hp.parities.iter().any(|x| x.is_some());
        assert!(exists_none);
        assert!(exists_some);
    }

    #[test]
    fn split_uneven_parities_give_two_equally_sized_vectors() {
        let str = String::from("hhhhhhh");
        let hp = HP::new(str)
            .get_parities();

        let (left, right) = hp.split_parities_in_half();

        assert!(left.len() == right.len());
    }

    #[test]
    fn right_split_parity_is_reversed() {
        let str = String::from("hphp");
        let hp = HP::new(str)
            .get_parities();
        
            let (_, right) = hp.split_parities_in_half();
            let first = right.get(0).unwrap();
            let second = right.get(1).unwrap();
            assert!(first.is_none());
            match second {
                Some(Parity::Even) => assert!(true),
                _ => assert!(false)
            }
    }
}