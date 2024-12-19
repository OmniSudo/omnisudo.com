/**
 * @author  omnisudo
 * @date    2024.12.08
 */

int main () {
    auto sq = skillquest::engine::client::Application();

    sq.mount( new skillquest::addon::base::client::AddonBaseCL() );
    sq.run();

    return 0;
}