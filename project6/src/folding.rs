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

use std::io::Read;

#[derive(Clone)]
enum Symbol {
    H,
    P,
}

#[derive(Clone, Debug, PartialEq)]
enum Parity {
    Even,
    Odd,
}

enum Direction {
    Left,
    Right,
}

#[derive(Clone)]
struct HP {
    symbols: Vec<Symbol>,
    parities: Vec<Option<Parity>>,
    matches: Matches,
}

impl HP {
    fn new(str: String) -> HP {
        let lower = str.to_lowercase();
        let len = str.len();
        if len % 2 != 0 {
            panic!(
                "Length of HP string should be an even number, was: {:?}",
                len
            );
        }
        let symbols = lower
            .chars()
            .map(|c| match c {
                'h' => Symbol::H,
                'p' => Symbol::P,
                _ => panic!("Unexpected character in HP string: {:?}", c),
            })
            .collect::<Vec<Symbol>>();
        HP {
            symbols: symbols,
            ..Default::default()
        }
    }

    fn get_parities(self) -> HP {
        let parities = self
            .symbols
            .iter()
            .enumerate()
            .map(|(i, s)| match s {
                Symbol::H => {
                    if i % 2 == 0 {
                        Some(Parity::Even)
                    } else {
                        Some(Parity::Odd)
                    }
                }
                Symbol::P => None,
            })
            .collect::<Vec<Option<Parity>>>();
        HP {
            symbols: self.symbols,
            parities: parities,
            ..Default::default()
        }
    }

    fn split_parities_in_half(
        parities: Vec<Option<Parity>>,
    ) -> (Vec<Option<Parity>>, Vec<Option<Parity>>) {
        let len = parities.len();
        let middle = len / 2;
        let (left, right) = {
            // Even
            let left = parities[0..middle].to_vec();
            let right = parities[middle..len].to_vec();
            (left, right)
        };

        // TODO: There must be some better way of doing this
        let mut mut_right = right;
        mut_right.reverse();
        let imut_right = (&*mut_right).to_vec();

        (left, imut_right)
    }

    fn get_maximum_matches(self) -> HP {
        let mut even_odd_matches = Matches::new();
        let mut odd_even_matches = Matches::new();

        let last_idx = self.parities.len() - 1;
        let parities_clone = self.parities.clone();
        let (left, right) = HP::split_parities_in_half(parities_clone);
        let zip = Iterator::zip(left.iter(), right.iter());
        zip.enumerate().for_each(|(idx, (l, r))| match (l, r) {
            (Some(Parity::Even), Some(Parity::Odd)) => {
                let l_idx = idx;
                let r_idx = last_idx - idx;
                even_odd_matches.left.push(l_idx);
                even_odd_matches.right.push(r_idx);
                even_odd_matches.amount += 1;
            }
            (Some(Parity::Odd), Some(Parity::Even)) => {
                let l_idx = idx;
                let r_idx = last_idx - idx;
                odd_even_matches.left.push(l_idx);
                odd_even_matches.right.push(r_idx);
                odd_even_matches.amount += 1;
            }
            _ => (),
        });

        if even_odd_matches.amount > odd_even_matches.amount {
            even_odd_matches.parity = Parity::Even;
            HP {
                symbols: self.symbols,
                parities: self.parities.clone(),
                matches: even_odd_matches,
            }
        } else {
            odd_even_matches.parity = Parity::Odd;
            HP {
                symbols: self.symbols,
                parities: self.parities.clone(),
                matches: odd_even_matches,
            }
        }
    }

    fn get_parities_for_split(hp: HP, split: usize) -> (usize, usize, usize, usize) {
        let mut left_evens = 0;
        let mut left_odds = 0;
        let mut right_evens = 0;
        let mut right_odds = 0;
        hp.parities
            .iter()
            .enumerate()
            .for_each(|(idx, parity)| -> () {
                match parity {
                    Some(Parity::Even) => {
                        if idx <= split {
                            left_evens += 1;
                        } else {
                            right_evens += 1;
                        }
                    }
                    Some(Parity::Odd) => {
                        if idx <= split {
                            left_odds += 1;
                        } else {
                            right_odds += 1;
                        }
                    }
                    _ => (),
                }
            });
        (left_evens, left_odds, right_evens, right_odds)
    }

    fn find_folding_point(hp: HP) -> usize {
        let mut split = Matches::get_middle_of_matches(hp.matches.clone());
        let (mut left_evens, mut left_odds, mut right_evens, mut right_odds) =
            HP::get_parities_for_split(hp.clone(), split);

        loop {
            if (left_evens >= right_evens && right_odds >= left_odds)
                || (right_evens >= left_evens && left_odds >= right_odds)
            {
                break;
            } else if left_evens >= right_evens && left_odds >= right_odds {
                // Go left
                // split = HP::update_folding_point(hp.clone(), split, Direction::Left);
                split -= 2;
            } else if right_evens >= left_evens && right_odds >= left_odds {
                // Go right
                // split = HP::update_folding_point(hp.clone(), split, Direction::Right);
                split += 2;
            }

            (left_evens, left_odds, right_evens, right_odds) =
                HP::get_parities_for_split(hp.clone(), split);
        }
        split
    }
    fn update_folding_point(hp: HP, split: usize, direction: Direction) -> usize {
        match direction {
            Direction::Left => {
                let left = hp.matches.left;
                let option = left.into_iter().rev().find(|idx| -> bool {
                    split > *idx 
                });
                match option {
                    Some(new_split) => new_split - 1,
                    None => panic!("Reached end of left array after searching for new split"),
                }
            }
            Direction::Right => {
                let right = hp.matches.right;
                let option = right.into_iter().rev().find(|idx| -> bool {
                    split < *idx 
                });
                match option {
                    Some(new_split) => new_split,
                    None => panic!("Reached end of right array after searching for new split"),
                }
            }
        }
    }

    fn create_substrings(hp: HP, split: usize) -> (Vec<Option<Parity>>, Vec<Option<Parity>>) {
        let len = hp.parities.len();
        let mut left = hp.parities[0..split+1].to_vec();
        let right = hp.parities[split+1..len].to_vec();

        left.reverse();

        (left.to_vec(), right)
    }

    fn make_fold(self, split: usize) {
        let (left_substring, right_substring) = HP::create_substrings(self.clone(), split);
        let matches = self.matches;

        loop {
            
        }
    }
}

impl Default for HP {
    fn default() -> HP {
        HP {
            symbols: Vec::new(),
            parities: Vec::new(),
            matches: Matches::new(),
        }
    }
}

#[derive(Clone)]
struct Matches {
    left: Vec<usize>,
    right: Vec<usize>,
    amount: usize,
    parity: Parity,
}

impl Matches {
    fn new() -> Matches {
        Matches {
            ..Default::default()
        }
    }

    fn get_middle_of_matches(matches: Matches) -> usize {
        let left_idx = matches.left.last().unwrap();
        let right_idx = matches.right.last().unwrap();
        let delta = right_idx - left_idx;
        let res = left_idx + (delta / 2);
        res
    }
}

impl Default for Matches {
    fn default() -> Matches {
        Matches {
            left: Vec::new(),
            right: Vec::new(),
            amount: 0,
            parity: Parity::Even,
        }
    }
}

#[cfg(test)]
mod tests {
    use crate::folding::Matches;
    use crate::folding::Parity;
    use crate::folding::HP;

    #[test]
    fn create_hp_struct_with_legal_lowercase_characters() {
        let str = String::from("hpphppphphpphphpphphphpphpphphphphpphp");
        let _ = HP::new(str);
        assert!(1 == 1); // dummy assert, method was successful if hp struct was created
    }

    #[test]
    fn create_hp_struct_with_legal_uppercase_characters() {
        let str = String::from("HPPPHPHPPPHPPPHPPPHPPPHHPPHPHPPHPH");
        let _ = HP::new(str);
        assert!(1 == 1); // dummy assert, method was successful if hp struct was created
    }

    #[test]
    fn create_hp_struct_with_legal_uppercase_and_lowercase_characters() {
        let str = String::from("HPPPHPHPPPHPhphpphhphpphpHPPPHHPHPHPHPPhphphhphp");
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
        let hp = HP::new(str).get_parities();

        hp.parities
            .iter()
            .for_each(|x| -> () { assert!(x.is_none()) });
    }

    #[test]
    fn parity_of_all_h_is_all_some() {
        let str = String::from("hhhhhhhhhhhhhhhhhh");
        let hp = HP::new(str).get_parities();

        hp.parities
            .iter()
            .for_each(|x| -> () { assert!(x.is_some()) });
    }

    #[test]
    fn parity_of_combo_is_both_some_and_none() {
        let str = String::from("hphphphpphpphpphhhpp");
        let hp = HP::new(str).get_parities();

        let exists_none = hp.parities.iter().any(|x| x.is_none());
        let exists_some = hp.parities.iter().any(|x| x.is_some());
        assert!(exists_none);
        assert!(exists_some);
    }

    #[test]
    fn right_split_parity_is_reversed() {
        let str = String::from("hphp");
        let hp = HP::new(str).get_parities();

        let (_, right) = HP::split_parities_in_half(hp.parities);
        let first = right.get(0).unwrap();
        let second = right.get(1).unwrap();
        assert!(first.is_none());
        match second {
            Some(Parity::Even) => assert!(true),
            _ => assert!(false),
        }
    }

    #[test]
    fn this_should_not_contain_any_matches() {
        let str = String::from("pppppppppppppp");
        let hp = HP::new(str).get_parities().get_maximum_matches();
        let (left, right) = (hp.matches.left, hp.matches.right);
        assert!(left.len() == 0);
        assert!(right.len() == 0);
    }

    #[test]
    fn should_contain_2_even_matches() {
        let str = String::from("hhhpphhh");
        let hp = HP::new(str).get_parities().get_maximum_matches();

        assert!(hp.matches.amount == 2);
        assert!(hp.matches.parity == Parity::Even);
    }
    #[test]
    fn should_contain_4_odd_matches() {
        let str = String::from("phhhhhhhhhhhhhhp");
        let hp = HP::new(str).get_parities().get_maximum_matches();

        assert!(hp.matches.amount == 4);
        assert!(hp.matches.parity == Parity::Odd);
    }

    #[test]
    fn should_split_on_index_3() {
        let str = String::from("hhhhhhhh");
        let hp = HP::new(str).get_parities().get_maximum_matches();

        let split = Matches::get_middle_of_matches(hp.matches);

        assert!(split == 3);
    }

    #[test]
    fn should_split_on_index_4() {
        let str = String::from("hhhhhhhhhh");
        let hp = HP::new(str).get_parities().get_maximum_matches();

        let split = Matches::get_middle_of_matches(hp.matches);

        assert!(split == 4);
    }

    #[test]
    fn should_have_2_even_on_left_side() {
        let str = String::from("hphhph");
        let hp = HP::new(str).get_parities().get_maximum_matches();
        let split = Matches::get_middle_of_matches(hp.clone().matches);
        let (left_evens, _, _, _) = HP::get_parities_for_split(hp.clone(), split);

        assert!(left_evens == 2);
    }

    #[test]
    fn should_have_3_odds_on_right_side() {
        let str = String::from("hphhhhhhhhph");
        let hp = HP::new(str).get_parities().get_maximum_matches();

        let split = Matches::get_middle_of_matches(hp.clone().matches);
        let (_, _, _, right_odds) = HP::get_parities_for_split(hp.clone(), split);

        assert!(right_odds == 3);
    }

    #[test]
    fn split_should_be_at_index_2() {
        let str = String::from("hhhhhh");
        let hp = HP::new(str).get_parities().get_maximum_matches();

        let split = HP::find_folding_point(hp.clone());

        assert!(split == 2);
    }

    #[test]
    fn split_should_be_at_index_3_after_moving() {
        let str = String::from("hhhhhhphppph");
        let hp = HP::new(str).get_parities().get_maximum_matches();

        let split = HP::find_folding_point(hp.clone());

        assert!(split == 3);
    }

    #[test]
    fn split_should_be_at_7_after_moving() {
        let str = String::from("hppphphhhhhh");
        let hp = HP::new(str).get_parities().get_maximum_matches();

        let split = HP::find_folding_point(hp.clone());
        assert!(split == 7);
    }

    #[test]
    fn split_should_be_at_12_after_moving() {
        let str = String::from("ppphppphphhhhhhhhh");
        let hp = HP::new(str).get_parities().get_maximum_matches();

        let split = HP::find_folding_point(hp.clone());

        assert!(split == 12);
    }
}
