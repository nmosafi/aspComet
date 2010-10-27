//
// These specs are attempting to capture the rules in section 2.2 of 
// http://svn.cometd.org/trunk/bayeux/bayeux.html
//

using System;
using Machine.Specifications;

namespace AspComet.Specifications
{
    [Subject("Channel patterns")]
    public class when_creating_one_which_does_not_start_with_slash
    {
        It should_fail_due_to_invalid_argument = () =>
            typeof(ArgumentException).ShouldBeThrownBy(() => new ChannelPattern("foo"));
    }

    [Subject("Channel patterns")]
    public class when_creating_one_which_has_wildcard_but_not_at_the_end
    {
        It should_fail_due_to_invalid_argument = () => 
            typeof(ArgumentException).ShouldBeThrownBy(() => new ChannelPattern("/*/foo"));
    }

    [Subject("Channel patterns")]
    public class when_creating_one_which_does_not_end_in_a_segment
    {
        It should_fail_due_to_invalid_argument = () => 
            typeof(ArgumentException).ShouldBeThrownBy(() => new ChannelPattern("/foo/"));
    }

    [Subject("Channel patterns")]
    public class when_a_single_segment_with_no_stars
    {
        static ChannelPattern pattern = new ChannelPattern("/foo");

        It should_match_with_the_same_string = () => pattern.ShouldMatch("/foo");
        It should_not_match_different_string = () => pattern.ShouldNotMatch("/bar");
    }

    [Subject("Channel patterns")]
    public class when_a_single_segment_as_a_single_star
    {
        static ChannelPattern pattern = new ChannelPattern("/*");

        It should_match_channel_name_with_single_segment = () => pattern.ShouldMatch("/foo");
        It should_not_match_channel_name_with_two_segments = () => pattern.ShouldNotMatch("/foo/bar");
    }

    [Subject("Channel patterns")]
    public class when_a_single_segment_as_a_double_star
    {
        static ChannelPattern pattern = new ChannelPattern("/**");

        It should_match_channel_name_with_single_segment = () => pattern.ShouldMatch("/foo");
        It should_not_match_channel_name_with_two_segments = () => pattern.ShouldMatch("/foo/bar");
    }

    [Subject("Channel patterns")]
    public class when_two_segments_with_no_wildcards
    {
        static ChannelPattern pattern = new ChannelPattern("/foo/bar");

        It should_match_the_same_string = () => 
            pattern.ShouldMatch("/foo/bar");

        It should_not_match_channelname_with_only_same_first_segment = () => 
            pattern.ShouldNotMatch("/foo");

        It should_not_match_channelname_with_same_first_segment_but_different_second_one = () => 
            pattern.ShouldNotMatch("/foo/boo");
    }

    [Subject("Channel patterns")]
    public class when_two_segments_with_single_star
    {
        static ChannelPattern pattern = new ChannelPattern("/foo/*");

        It should_match_channelname_with_first_segment_and_any_second_one = () => 
            pattern.ShouldMatch("/foo/bar");

        It should_not_match_channelname_with_first_segment_and_any_second_one_plus_an_extra_one = () => 
            pattern.ShouldNotMatch("/foo/bar/moo");
    }

    [Subject("Channel patterns")]
    public class when_two_segments_with_double_star
    {
        static ChannelPattern pattern = new ChannelPattern("/foo/**");

        It should_match_channelname_with_first_segment_and_any_second_one = () => 
            pattern.ShouldMatch("/foo/bar");

        It should_match_channelname_with_first_segment_and_any_second_one_plus_an_extra_one = () => 
            pattern.ShouldMatch("/foo/bar/moo");
    }

    public static class ChannelPatternExtensions
    {
        public static void ShouldMatch(this ChannelPattern pattern, string match)
        {
            pattern.Matches(match).ShouldBeTrue();
        }

        public static void ShouldNotMatch(this ChannelPattern pattern, string match)
        {
            pattern.Matches(match).ShouldBeFalse();
        }
    }
}
