#include <gtest/gtest.h>
#include "../src/config.h"

using namespace shanghai;

const char * const TestSrc = R"(
{
	"testval": { "a": true, "b": 42, "c": "str" },
	"long": {"a": {"a": {"a": {"a": {"a": {"a": {"a": {"a": {"a": 7}}}}}}}}}
}
)";

TEST(ConfigTest, Parse) {
	Config config;
	config.LoadString(TestSrc);
}

TEST(ConfigTest, Bool) {
	Config config;
	config.LoadString(TestSrc);
	EXPECT_TRUE(config.GetBool({"testval", "a"}));
}

TEST(ConfigTest, Int) {
	Config config;
	config.LoadString(TestSrc);
	EXPECT_EQ(42, config.GetInt({"testval", "b"}));
}

TEST(ConfigTest, String) {
	Config config;
	config.LoadString(TestSrc);
	EXPECT_EQ("str", config.GetStr({"testval", "c"}));
}

TEST(ConfigTest, LongKey) {
	Config config;
	config.LoadString(TestSrc);
	EXPECT_EQ(7, config.GetInt({"long",
		"a", "a", "a", "a", "a", "a", "a", "a", "a"}));
}

TEST(ConfigTest, KeyError) {
	Config config;
	config.LoadString(TestSrc);
	EXPECT_THROW(config.GetBool({"testval", "invalkey"}), ConfigError);
}

const char * const TestSrc1 = R"(
{
	"x": { "a": true, "b": 42, "c": "str" },
	"y": { "a": { "b": 7 }}
}
)";
const char * const TestSrc2 = R"(
{
	"x": { "a": false },
	"y": { "a": { "c": 49 }}
}
)";

TEST(ConfigTest, Multi) {
	Config config;
	config.LoadString(TestSrc1);
	config.LoadString(TestSrc2);

	EXPECT_FALSE(config.GetBool({"x", "a"}));
	EXPECT_EQ(42, config.GetInt({"x", "b"}));
	EXPECT_EQ("str", config.GetStr({"x", "c"}));

	EXPECT_EQ(7, config.GetInt({"y", "a", "b"}));
	EXPECT_EQ(49, config.GetInt({"y", "a", "c"}));
}
